using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using org.secc.Rock.DataImport.BAL.Integration;
using org.secc.Rock.DataImport.Extensions.Arena.Model;

namespace org.secc.Rock.DataImport.Extensions.Arena
{
    
    [Export(typeof(IIntegrationComponent))]
    [ExportMetadata("Name", "Arena")]
    [ExportMetadata("Description", "Imports Data from ArenaChMS to RockRMS through a SQL Database connection. This integration is based on the database schema for ArenaChMS 2013.1.100")]
    public class ArenaIntegration : IIntegrationComponent 
    {
        [ImportMany(ArenaIntegration.IDENTIFIER, typeof(iExportMapComponent))]
        public List<Lazy<iExportMapComponent, iExportMapData>> ExportMaps { get; set; }

        public const string IDENTIFIER = "Arena";

        public bool TestConnection(Dictionary<string,string> connectSettings, out string errorMessage)
        {
            bool isSuccessful = true;
            errorMessage = null;

            try
            {
                using ( ArenaContext context = ArenaContext.BuildContext(connectSettings) )
                {
                    int peopleCount = context.Person.Count();

                }
            }
            catch ( Exception ex )
            {
                errorMessage = "Unable to connect to database. Please verify connection settings and retry.";
                isSuccessful = false;
            }

            return isSuccessful;

        }

        public IntegrationConnectionControl GetConnectionControl()
        {
            return new ConnectionSettings();
        }


    }
}
