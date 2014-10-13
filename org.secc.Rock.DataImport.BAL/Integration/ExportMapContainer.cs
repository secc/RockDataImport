using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                        Component = c.Value,
                        ImportRanking = c.Metadata.ImportRanking
                    } ).ToList();
        }
    }

    public class ExportMap
    {
        #region Fields
        private int mImportRanking = 0;
        private bool mSelected = false;
        private ExportStatus mStatus = ExportStatus.NotSelected;
        #endregion

        #region Properties
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public iExportMapComponent Component { get; set; }

        public int ImportRanking
        {
            get
            {
                return mImportRanking;
            }
            set
            {
                mImportRanking = value;
            }
        }
        
        public bool Selected
        {
            get
            {
                return mSelected;

            }
            set
            {
                mSelected = value;

                if ( mSelected )
                {
                    Status = ExportStatus.Waiting;
                }
                else
                {
                    Status = ExportStatus.NotSelected;
                }
            }
        }

        public ExportStatus Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
            }
        }

        public string StatusDescription
        {
            get
            {
                Type type = typeof(ExportStatus);
                var memInfo = type.GetMember( Status.ToString() ).FirstOrDefault();
                var attributes = memInfo.GetCustomAttributes( typeof( DescriptionAttribute ), false );
                //string description = ( (DescriptionAttribute)attributes[0] ).Description;

                if ( attributes.Count() > 0)
                {
                    return ( (DescriptionAttribute)attributes[0] ).Description;
                }
                else
                {
                    return Status.ToString();
                }

            }
        }

        #endregion

        public enum ExportStatus
        {
            [Description("Not Selected")]
            NotSelected,
            Waiting,
            Importing,
            Completed,
            Failed,
            Cancelled
        }
    }


}
