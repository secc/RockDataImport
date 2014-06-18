using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Integration;

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
            Dictionary<string, object> rockCampus = rockCampusMap.GetByForeignId( campusId.ToString() );

            if ( rockCampus == null )
            {
                int? rockCampusId = rockCampusMap.SaveCampus( isSystem: false, name: arenaCampus.name, foreignId: arenaCampus.campus_id.ToString() );
                if ( rockCampusId == null )
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }
            }

            OnExportAttemptCompleted( identifier, true );        

        }

        public List<string> GetDependencies()
        {
            return System.Attribute.GetCustomAttributes( this.GetType() )
                    .Where( a => a.GetType() == typeof( DependencyAttribute ) )
                    .OrderBy(a => a.GetType().GetProperties().Where(p => p.Name == "Order").Select(p => (int)p.GetValue(a)).FirstOrDefault())
                    .Select( a => a.GetType().GetProperties().Where( p => p.Name == "FriendlyName" ).Select( p => p.GetValue( a ) ).FirstOrDefault().ToString() ).ToList();


                        
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
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
                return context.Campus.FirstOrDefault( c => c.campus_id == campusId );
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
    }
}
