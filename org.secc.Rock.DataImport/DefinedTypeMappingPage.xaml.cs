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


using org.secc.Rock.DataImport.BAL.Integration;
using org.secc.Rock.DataImport.BAL.Helper;

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for DefinedTypeMappingPage.xaml
    /// </summary>
    public partial class DefinedTypeMappingPage : Page
    {
        ExportIntegrations Integration { get; set; }

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

        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {

        }

        #endregion

        #region Private 

        private void LoadDefinedTypes()
        {

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
