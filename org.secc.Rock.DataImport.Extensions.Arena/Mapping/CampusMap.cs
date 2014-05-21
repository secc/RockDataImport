using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Integration;

namespace org.secc.Rock.DataImport.Extensions.Arena.Mapping
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Campus")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    public class CampusMap : iExportMapComponent
    {
        private int? mRecordCount;

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
                            .Skip( startingRecord );

                if ( size > 0 )
                {
                    query = query.Take( size );
                }

                return query.Select( c => c.campus_id.ToString() ).ToList();
            }
        }

        public void ExportRecord( string identifier )
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> OnExportSuccess;

        public event EventHandler<EventArgs> OnExportFailure;


        private int? GetRecordCount()
        {
            using ( Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.Campus.Count();
            }
        }
    }
}
