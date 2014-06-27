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
    [Dependency("Person", typeof(PersonMap), 10)]
    [Dependency("person2", typeof(PersonMap), 0)]
    public class CampusMap : iExportMapComponent
    {
        private int? mRecordCount;
        private int? mDefinedTypeCount;

        private Dictionary<string,string> ConnectionInfo{get;set;}

        public int? RecordCount
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

        public int? DefinedTypeCount
        {
            get
            {
                if(mDefinedTypeCount == null)
                {
                    mDefinedTypeCount = GetDefinedTypeCount();
                }

                return mDefinedTypeCount;
            }
        }

        private CampusMap() { }

        [ImportingConstructor]
        public CampusMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo )
        {
            ConnectionInfo = connectionInfo;
        }

        public List<string> GetSubsetIDs( int startingRecord = 0, int size = 0 )
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

        public void ExportRecord( string identifier, RockService service  )
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

            BAL.RockMaps.CampusMap rockCampusMap = new BAL.RockMaps.CampusMap( service );
            int? rockCampusId = null;
            Dictionary<string, object> rockCampus = rockCampusMap.GetByForeignId( campusId.ToString() );

            if ( rockCampus == null )
            {
                int? locationId = null;

                if ( arenaCampus.address_id != null )
                {
                    locationId = SaveCampusLocation( arenaCampus.name, arenaCampus.Address, service );
                }
                int? leaderPersonId = null;
                //if ( arenaCampus.leader_person_id != null )
                //{
                //    var leaderTask = SaveCampusLeaderPerson( arenaCampus.leader_person_id.ToString(), service );
                //    leaderPerson = leaderTask.Result;
                //}

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

        public List<string> GetDependencies()
        {
            return System.Attribute.GetCustomAttributes( this.GetType() )
                    .Where( a => a.GetType() == typeof( DependencyAttribute ) )
                    .OrderBy(a => a.GetType().GetProperties().Where(p => p.Name == "Order").Select(p => (int)p.GetValue(a)).FirstOrDefault())
                    .Select( a => a.GetType().GetProperties().Where( p => p.Name == "FriendlyName" ).Select( p => p.GetValue( a ) ).FirstOrDefault().ToString() ).ToList();


                        
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if(handler != null)
            {
                handler( this, args );
            }
        }

        public event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;


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

        private int? GetDefinedTypeCount()
        {
            return System.Attribute.GetCustomAttributes( this.GetType() )
                .Where( a => a.GetType() == typeof( DefinedTypeAttribute ) ).Count();
        }

        private int? SaveCampusLocation( string campusName, Model.Address a, RockService service )
        {
            RockMap.LocationMap locationMap = new RockMap.LocationMap( service );

            Dictionary<string,object> rockLocation = locationMap.GetByForeignId( a.address_id.ToString() );

            if ( rockLocation != null )
            {
                return (int)rockLocation["Id"];
            }

            int? addressid = locationMap.SaveAddress( a.street_address_1, a.city, a.state, a.country, a.postal_code, a.street_address_2, a.Latitude,
                a.Longitude, a.address_id.ToString(), campusName, true, null, null, locationMap.GetCampusLocationDefinedValueId() );


            return addressid;
           
        }

        private Task<int?> SaveCampusLeaderPerson( string personId, RockService service )
        {
            var tcs = new TaskCompletionSource<int?>();
            var PersonMap = new PersonMap( ConnectionInfo );

            PersonMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );

            PersonMap.ExportRecord( personId, service );

            return tcs.Task;

        }
    }
}
