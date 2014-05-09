using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.secc.Rock.DataImport.BAL.Integration
{

    public interface IIntegrationComponent
    {
        List<Lazy<iExportMapComponent, iExportMapData>> ExportMaps { get;set; }
        bool TestConnection(out string errorMessage);

    }
}
