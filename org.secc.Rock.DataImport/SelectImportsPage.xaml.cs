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

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for SelectImportsPage.xaml
    /// </summary>
    public partial class SelectImportsPage : Page
    {
        public ExportIntegrations Integration { get; set; }


        private SelectImportsPage()
        {
            InitializeComponent();
        }

        public SelectImportsPage( ExportIntegrations integration )
        {
            Integration = integration;
            InitializeComponent();
        }

        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            if ( Integration != null )
            {
                grdEntities.DataContext = Integration.Component.ExportMaps;
            }
        }

        private void grdEntities_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {

        }

        private void btnBack_Click( object sender, RoutedEventArgs e )
        {
            if ( this.NavigationService.CanGoBack )
            {
                this.NavigationService.GoBack();
            }
        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {
            int selectedMaps = Integration.Component.ExportMaps.Count( m => m.Selected );

            SetAlertMessage( string.Format( "{0} selected", selectedMaps ) );
        }


        private void SetAlertMessage( string message )
        {
            lblAlert.Content = message;
            lblAlert.Visibility = String.IsNullOrWhiteSpace( message ) ? Visibility.Collapsed : Visibility.Visible;
        }

    }


}
