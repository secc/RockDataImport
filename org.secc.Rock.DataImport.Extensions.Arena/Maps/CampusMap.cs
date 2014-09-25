using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Integration;
using RockMap = org.secc.Rock.DataImport.BAL.RockMaps;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Campus")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "Campus locations from ArenaChMS.")]
    public class CampusMap : ArenaMapBase
    {

        public override int? RecordCount
        {
            get
            {
                if ( mRecordCount == null )
                {
                    mRecordCount = GetRecordCount();
                }

                return mRecordCount;
            }
        }

        private CampusMap() : base( typeof( CampusMap ) ) { }

        [ImportingConstructor]
        public CampusMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo, [Import("RockService")]RockService service ) : base( typeof( CampusMap ), connectionInfo, service)
        { }

        public override List<string> GetSubsetIDs( int startingRecord = 0, int size = 0 )
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                var query = Context.Campus
                            .OrderBy(c => c.campus_id)
                            .Skip( startingRecord );

                if ( size > 0 )
                {
                    query = query.Take( size );
                }

                return query.Select( c => c.campus_id.ToString() ).ToList();
            }
        }

        public override void ExportRecord( string identifier )
        {
            int campusId = 0;
            if ( !int.TryParse( identifier, out campusId ) )
            {
                OnExportAttemptCompleted( identifier, false );
                return;
            }

            Model.Campus arenaCampus = GetArenaCampus( campusId );

            if ( arenaCampus == null )
            {
                OnExportAttemptCompleted( identifier, false );
                return; 
            }

            BAL.RockMaps.CampusMap rockCampusMap = new BAL.RockMaps.CampusMap( Service );
            int? rockCampusId = null;
            Dictionary<string, object> rockCampus = rockCampusMap.GetByForeignId( campusId.ToString() );

            if ( rockCampus == null )
            {
                int? locationId = null;

                if ( arenaCampus.address_id != null )
                {
                    locationId = SaveCampusLocation( arenaCampus.name, arenaCampus.Address );
                }
                int? leaderPersonId = null;

                rockCampusId = rockCampusMap.SaveCampus( isSystem: false, name: arenaCampus.name, foreignId: arenaCampus.campus_id.ToString(), locationId: locationId, leaderPersonAliasId: leaderPersonId );
                if ( rockCampusId == null )
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }
            }
            else
            {
                rockCampusId = (int?)rockCampus["Id"];
            }

            OnExportAttemptCompleted( identifier, true, rockCampusId );        

        }

        public override Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType )
        {
            return GetAttributes( this.GetType(), attributeType );

        }


        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null, Type mapType = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            if ( mapType == null )
            {
                args.MapType = this.GetType();
            }
            else
            {
                args.MapType = mapType;
            }

            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if(handler != null)
            {
                handler( this, args );
            }
        }

        public override event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        private Model.Campus GetArenaCampus( int campusId )
        {
            using ( Model.ArenaContext context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.Campus.Include("Address").FirstOrDefault( c => c.campus_id == campusId );
            }
        }

        private int? GetRecordCount()
        {
            using ( Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.Campus.Count();
            }
        }

        private int? SaveCampusLocation( string campusName, Model.Address a )
        {
            RockMap.LocationMap locationMap = new RockMap.LocationMap( Service );

            Dictionary<string,object> rockLocation = locationMap.GetByForeignId( a.address_id.ToString() );

            if ( rockLocation != null )
            {
                return (int)rockLocation["Id"];
            }

            int? addressid = locationMap.SaveAddress( a.street_address_1, a.city, a.state, a.country, a.postal_code, a.street_address_2, a.Latitude,
                a.Longitude, a.address_id.ToString(), campusName, true, null, null, locationMap.GetCampusLocationDefinedValueId() );


            return addressid;
           
        }

        private Task<int?> SaveCampusLeaderPerson( string personId )
        {
            var tcs = new TaskCompletionSource<int?>();
            var PersonMap = new PersonMap( ConnectionInfo, Service );

            PersonMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );

            PersonMap.ExportRecord( personId );

            return tcs.Task;

        }
    }
}
