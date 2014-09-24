using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public class ExportMapContainer
    {
        [ImportMany( typeof( iExportMapComponent ) )]
        protected List<Lazy<iExportMapComponent, iExportMapData>> Components;
        private CompositionContainer container;

        private ExportMapContainer()
        {
           // new ExportMapContainer( System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Plugins" ) );
        }

        public ExportMapContainer( Dictionary<string, string> connectionInfo, RockService service )
        {
            string pluginPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            LoadComponents( connectionInfo, pluginPath, service );
        }

        public ExportMapContainer( Dictionary<string, string> connectionInfo, string pluginPath, RockService service )
        {
            LoadComponents( connectionInfo, pluginPath, service );
        }

        private void LoadComponents( Dictionary<string,string> connectionInfo, string pluginFolder, RockService service )
        {
            Components = new List<Lazy<iExportMapComponent, iExportMapData>>();
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add( new AssemblyCatalog( this.GetType().Assembly ) );

            if ( System.IO.Directory.Exists( pluginFolder ) )
            {
                catalog.Catalogs.Add( new SafeDirectoryCatalog( pluginFolder ) );
            }
            container = new CompositionContainer( catalog );
            container.ComposeExportedValue( "ConnectionInfo", connectionInfo );
            container.ComposeExportedValue( "RockService", service );
            container.ComposeParts( this );


        }

        public List<ExportMap> GetExportMaps(string integrationIdentifier)
        {
            return Components.Where( c => c.Metadata.Integration == integrationIdentifier )
                    .Select( c => new ExportMap
                    {
                        Name = c.Metadata.Name,
                        Description = c.Metadata.Description,
                        Component = c.Value
                    } ).ToList();
        }
    }

    public class ExportMap
    {
        #region Fields
        private bool mSelected = false;
        #endregion

        #region Properties
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public iExportMapComponent Component { get; set; }
        
        public bool Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                mSelected = value;
            }
        }
        #endregion
    }
}
