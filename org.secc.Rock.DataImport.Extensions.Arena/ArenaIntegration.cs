using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Integration;

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

        public bool TestConnection(out string errorMessage)
        {
            string connectionString = "Data Source=p10-ardevsql01;Initial Catalog=ArenaDB_ChrisF;Integrated Security=SSPI;MultipleActiveResultSets=true;";
            bool isSuccessful = true;
            errorMessage = null;

            try
            {
                using ( Model.ArenaContext context = new Model.ArenaContext( connectionString ) )
                {
                    int peopleCount = context.Person.Count();

                }
            }
            catch ( Exception ex )
            {
                errorMessage = ex.Message;
                isSuccessful = false;
            }

            return isSuccessful;

        }



    }
}
