using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Integration
{
    public class IntegrationContainer
    {
        [ImportMany( typeof( IIntegrationComponent ) )]
        protected List<Lazy<IIntegrationComponent, IIntegrationData>> Components = new List<Lazy<IIntegrationComponent, IIntegrationData>>();

        public IntegrationContainer() 
        {
            new IntegrationContainer( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Plugins" ) );
        }

        public IntegrationContainer( string pluginFolder )
        {
            var catalog = new AggregateCatalog();

            if ( Directory.Exists( pluginFolder ) )
            {
                catalog.Catalogs.Add( new SafeDirectoryCatalog( pluginFolder ) );
            }

            catalog.Catalogs.Add( new AssemblyCatalog( this.GetType().Assembly ) );

            try
            {
                var container = new CompositionContainer( catalog );
                container.ComposeParts( this );
            }
            catch(Exception ex)
            {
                throw new IntegrationLoadException( "An exception occurred while loading integrations.", ex );
            }
        }

        public Dictionary<string, KeyValuePair<IIntegrationComponent, string>> Dictionary()
        {
            Dictionary<string, KeyValuePair<IIntegrationComponent, string>> componentDictionary = new Dictionary<string, KeyValuePair<IIntegrationComponent, string>>();

            foreach ( var c in Components )
            {
                componentDictionary.Add( c.Metadata.Name, new KeyValuePair<IIntegrationComponent, string>( c.Value, c.Metadata.Description ) );
            }

            return componentDictionary;
        }

        public List<ExportIntegrations> GetIntegrations()
        {
            List<ExportIntegrations> iList = new List<ExportIntegrations>();

            foreach ( var c in Components )
            {
                iList.Add( new ExportIntegrations() { Name = c.Metadata.Name, Component = c.Value, Description = c.Metadata.Description } );
            }

            return iList;
        }

    }

    public class ExportIntegrations
    {
        public string Name { get; set; }
        public IIntegrationComponent Component { get; set; }
        public string Description { get; set; }

    }

    [Serializable]
    public class IntegrationLoadException : Exception
    {
        public IntegrationLoadException() { }
        public IntegrationLoadException( string message ) : base( message ) { }
        public IntegrationLoadException( string message, Exception inner ) : base( message, inner ) { }
        protected IntegrationLoadException( 
	    System.Runtime.Serialization.SerializationInfo info, 
	    System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}
