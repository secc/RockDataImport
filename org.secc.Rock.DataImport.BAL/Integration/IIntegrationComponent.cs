using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.secc.Rock.DataImport.BAL.Integration
{

    public interface IIntegrationComponent
    {
        string Identifier { get; }
        List<ExportMap> ExportMaps { get; }
        string PluginFolder { get; set; }
        Dictionary<string, string> ConnectionInfo { get; set; }

        bool TestConnection(Dictionary<string,string> connectionSettings, out string errorMessage);
        IntegrationConnectionControl GetConnectionControl();
    }
}
