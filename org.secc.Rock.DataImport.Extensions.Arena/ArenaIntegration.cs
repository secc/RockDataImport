using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        #region Fields
        public const string IDENTIFIER = "Arena";
        private List<ExportMap> mExportMaps = null;
        #endregion

        #region Properties
        public List<ExportMap> ExportMaps
        {
            get
            {
                if ( mExportMaps == null )
                {
                    mExportMaps = LoadExportMaps();
                }

                return mExportMaps;
            }
        }

        /// <summary>
        /// Returns the Identifier constant string so that it can be used in the base application
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the integration's identifier key.
        /// </value>
        public string Identifier
        {
            get
            {
                return IDENTIFIER;
            }
        }

        public Dictionary<string, string> ConnectionInfo { get; set; }

        public  string PluginFolder { get; set; }
        #endregion


        #region Constructor
        public ArenaIntegration() { }

        [ImportingConstructor]
        public ArenaIntegration( [Import( "PluginFolder" )] string pluginFolder )
        {
            if ( System.IO.Directory.Exists( pluginFolder ) )
            {
                PluginFolder = pluginFolder;
            }
        }
        #endregion

        #region Public Methods
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

        #endregion

        #region Private Methods
        private List<ExportMap> LoadExportMaps()
        {
            ExportMapContainer container = new ExportMapContainer(ConnectionInfo, PluginFolder );
            return container.GetExportMaps( Identifier );
        }
        #endregion

    }
}
