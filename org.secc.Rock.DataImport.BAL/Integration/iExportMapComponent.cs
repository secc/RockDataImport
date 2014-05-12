using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public interface iExportMapComponent
    {
        //int? RecordCount { get; }
        List<string> GetSubsetIDs( Dictionary<string,string> connection, int startingRecord, int size );
        void ExportRecord( Dictionary<string,string> connection,  string identifier );
        int? GetRecordCount( Dictionary<string, string> connection );

        event EventHandler<EventArgs> OnExportSuccess;
        event EventHandler<EventArgs> OnExportFailure;
        

    }
}
