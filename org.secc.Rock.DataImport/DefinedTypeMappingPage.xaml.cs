using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Integration;
using org.secc.Rock.DataImport.BAL.Helper;
using org.secc.Rock.DataImport.BAL.RockMaps;

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for DefinedTypeMappingPage.xaml
    /// </summary>
    public partial class DefinedTypeMappingPage : Page
    {
        ExportIntegrations Integration { get; set; }
        List<MappedDefinedType> MappedDefinedTypes = null;


        private DefinedTypeMappingPage()
        {
            InitializeComponent();
        }

        public DefinedTypeMappingPage( ExportIntegrations integration )
        {
            Integration = integration;
            InitializeComponent();
        }


        #region Page Events
        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            LoadDefinedTypes();
        }

        private void btnBack_Click( object sender, RoutedEventArgs e )
        {
            if ( NavigationService.CanGoBack )
            {
                NavigationService.GoBack();
            }
        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {

        }

        #endregion

        #region Private 

        public DefinedTypeSummary GetRockDefinedType( string rockGuidString )
        {
            DefinedTypeMap dtm = new DefinedTypeMap( App.RockService );
            DefinedTypeSummary summary = null;

            Guid rockGuid;

            if ( Guid.TryParse( rockGuidString, out rockGuid ) )
            {
                summary = dtm.GetDefinedTypeSummary( rockGuid );
            }

            return summary;
        }

        public DefinedTypeSummary GetRockDefinedTypeByForeignKey( string sourceForeignId )
        {
            DefinedTypeMap dtm = new DefinedTypeMap( App.RockService );

            return dtm.GetDefinedTypeSummaryByForeignId( sourceForeignId );

        }

        public DefinedTypeSummary GetSourceDefinedType( string sourceId )
        {
            return Integration.Component.GetDefinedTypeSummary( sourceId );
        }

        /// <summary>
        /// Loads the defined types that are referenced in the defined type attributes for the selected maps into the MappedDefinedTypes list.
        /// </summary>
        private void LoadDefinedTypes()
        {
            MappedDefinedTypes = new List<MappedDefinedType>();
            foreach ( var map in Integration.Component.ExportMaps.Where(m => m.Selected ) )
            {
                LoadDefinedTypes( map.Component );
            }

            foreach ( var mappedDT in MappedDefinedTypes )
            {
                mappedDT.SourceDefinedTypeSummary = GetSourceDefinedType( mappedDT.SourceId );

                if(!String.IsNullOrWhiteSpace(mappedDT.RockGuid))
                {
                    mappedDT.RockDefinedTypeSummary = GetRockDefinedType( mappedDT.RockGuid );

                }

                if ( mappedDT.RockDefinedTypeSummary == null && mappedDT.SourceDefinedTypeSummary != null )
                {
                    mappedDT.RockDefinedTypeSummary = GetRockDefinedType( mappedDT.SourceDefinedTypeSummary.Id );
                }
            }
        }

        /// <summary>
        /// Loads the distinct defined types for the export map and the maps that it is dependent on 
        /// </summary>
        /// <param name="mapComponent">The export map.</param>
        private void LoadDefinedTypes( iExportMapComponent mapComponent )
        {

            foreach ( var attribute in mapComponent.GetAttributes(typeof(DefinedTypeAttribute)) )
            {
                if ( MappedDefinedTypes.Where( dt => dt.SourceId == attribute.Value["SourceDefinedTypeIdentifier"].ToString() )
                        .Where( dt => dt.RockGuid == attribute.Value["RockDefinedTypeGuid"].ToString() )
                        .Count() == 0 )
                {
                    MappedDefinedTypes.Add( new MappedDefinedType()
                            {
                                Name = attribute.Key,
                                SourceId = attribute.Value["SourceDefinedTypeIdentifier"].ToString(),
                                RockGuid = attribute.Value["RockDefinedTypeGuid"].ToString(),
                                SourceDefinedTypeSummary = null,
                                RockDefinedTypeSummary = null
                            } );
                }
            }

            var dependantMaps = Integration.Component.ExportMaps
                                    .Where( m => !m.Selected )
                                    .Where( m => mapComponent.GetAttributes( typeof( DependencyAttribute ) ).Select( da => ( (Type)da.Value["Dependency"] ).ToString() ).Contains( m.Component.GetType().ToString() ) )
                                    .Select( m => m.Component );

            foreach ( var map in dependantMaps )
            {
                LoadDefinedTypes( map );
            }
        }

        #endregion

        internal class MappedDefinedType
        {
            internal string Name { get; set; }
            internal string SourceId { get; set; }
            internal string RockGuid { get; set; }
            internal DefinedTypeSummary SourceDefinedTypeSummary { get; set; }
            internal DefinedTypeSummary RockDefinedTypeSummary { get; set; }

            internal bool IsMapped
            {
                get
                {
                    return ValueIsMapped();
                }
            }


            private bool ValueIsMapped()
            {
                bool mapped = false;

                if ( SourceDefinedTypeSummary != null && SourceDefinedTypeSummary.ValueSummaries != null )
                {
                    int valuesCount = SourceDefinedTypeSummary.ValueSummaries.Count;

                    if ( valuesCount > 0 && SourceDefinedTypeSummary.ValueSummaries.Where( vs => !String.IsNullOrWhiteSpace( vs.ForeignId ) ).Count() == valuesCount )
                    {
                        mapped = true;
                    }
                }

                return mapped;
            }

        }
    }
}
