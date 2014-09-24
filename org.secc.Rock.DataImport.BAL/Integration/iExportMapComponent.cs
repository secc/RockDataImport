using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public interface iExportMapComponent
    {
        int? RecordCount { get; }
        List<string> GetSubsetIDs( int startingRecord, int size );
        //List<string> GetDependencies();
        void ExportRecord( string identifier );
        Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType );
        event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;
    }
}
