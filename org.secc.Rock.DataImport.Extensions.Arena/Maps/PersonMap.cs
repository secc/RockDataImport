using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Integration;


namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Person")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "People records from ArenaChMS")]
    public class PersonMap : iExportMapComponent
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

        private PersonMap() {}

        [ImportingConstructor]
        public PersonMap([Import("ConnectionInfo")] Dictionary<string,string> connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }

        public List<string> GetSubsetIDs( int startingRecord, int size )
        {
            throw new NotImplementedException();
        }

        public void ExportRecord( string identifier, RockService service)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        private int? GetRecordCount()
        {
            using (Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext(ConnectionInfo))
            {
                return Context.Person.Count();
            }
        }
    }
}
