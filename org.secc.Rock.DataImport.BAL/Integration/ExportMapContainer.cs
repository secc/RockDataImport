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

        public ExportMapContainer( Dictionary<string, string> connectionInfo )
        {
            string pluginPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            LoadComponents( connectionInfo, pluginPath );
        }

        public ExportMapContainer( Dictionary<string, string> connectionInfo, string pluginPath )
        {
            LoadComponents( connectionInfo, pluginPath );
        }

        private void LoadComponents( Dictionary<string,string> connectionInfo, string pluginFolder )
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
            container.ComposeParts( this );


        }

        public Dictionary<string, iExportMapComponent> GetExportMaps(string integrationIdentifier)
        {
            return Components.Where( c => c.Metadata.Integration == integrationIdentifier ).ToDictionary( c => c.Metadata.Name, c => c.Value );
        }
    }
}
