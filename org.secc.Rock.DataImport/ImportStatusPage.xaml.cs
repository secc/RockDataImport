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
    /// Interaction logic for ImportStatusPage.xaml
    /// </summary>
    public partial class ImportStatusPage : Page
    {
        ExportIntegrations Integration;

        private ImportStatusPage()
        {
            InitializeComponent();
        }

        public ImportStatusPage( ExportIntegrations integration )
        {
            Integration = integration;
            InitializeComponent();
        }

        #region Page Events
        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            SetStartVisibility( true );
            SetProgressVisibility( false );
        }

        private void btnBack_Click( object sender, RoutedEventArgs e )
        {
            if ( NavigationService.CanGoBack )
            {
                NavigationService.GoBack();
            }
        }

        private void btnFinish_Click( object sender, RoutedEventArgs e )
        {

        }

        private void btnBegin_Click( object sender, RoutedEventArgs e )
        {
            StartImportProcess();
        }

        private void btnCancel_Click( object sender, RoutedEventArgs e )
        {

        }
        #endregion

        #region Private Methods

        private void BindMapGrid()
        {
            grdMaps.ItemsSource =  Integration.Component.ExportMaps.OrderByDescending( m => m.ImportRanking ).ThenBy( m => m.Name );
            grdMaps.Items.Refresh();

        }

        private void SetProgressVisibility( bool display )
        {
            if ( display )
            {
                grdContent.Visibility = System.Windows.Visibility.Visible;
                btnCancel.Visibility = System.Windows.Visibility.Visible;
                btnFinish.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                grdContent.Visibility = System.Windows.Visibility.Hidden;
                btnCancel.Visibility = System.Windows.Visibility.Hidden;
                btnFinish.Visibility = System.Windows.Visibility.Hidden;

            }
        }

        private void SetStartVisibility( bool display )
        {
            if ( display )
            {
                tbStartMessage.Visibility = System.Windows.Visibility.Visible;
                btnBegin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tbStartMessage.Visibility = System.Windows.Visibility.Hidden;
                btnBegin.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void StartImportProcess()
        {
            BindMapGrid();
            SetStartVisibility( false );
            SetProgressVisibility( true );
        
        }



        #endregion
    }
}
