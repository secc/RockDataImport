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
        int? DefinedTypeCount { get; }
        List<string> GetSubsetIDs( int startingRecord, int size );
        //List<string> GetDependencies();
        void ExportRecord( string identifier, RockService service );

        event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;
    }
}
