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
    /// Interaction logic for ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Page
    {

        List<ExportIntegrations> Integrations = null;
        IntegrationConnectionControl ConnectionControl = null;
        public ConfigurationPage()
        {
            InitializeComponent();

        }

        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            GetIntegrations();
            BindDataSourceList();
        }

        private void cboDataSource_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            bool controlsVisible = false;
            if ( cboDataSource.SelectedIndex > 0 )
            {
                controlsVisible = LoadIntegrationConnectionControl( cboDataSource.SelectedValue.ToString() );
            }
            else
            {
                ClearConnectionControl();
            }

            SetControlVisibility( controlsVisible );
        }

        private void btnTestConnection_Click( object sender, RoutedEventArgs e )
        {
            if ( TestConnection() )
            {
                SetSuccessMessage( "Connection Successful!" );
            }
        }

        private void BindDataSourceList()
        {
            cboDataSource.Items.Clear();
            if ( Integrations.Count > 0 )
            {
                //cboDataSource.ItemsSource = Integrations.Select( i => i.Name );
                cboDataSource.Items.Insert( 0, "--Select--" );

                foreach ( var i in Integrations )
                {
                    cboDataSource.Items.Add( i.Name );
                }
                cboDataSource.SelectedIndex = 0;
            }
            else
            {
                SetWarningMessage( string.Format( "No integrations found. Please verify that integrations have been loaded in the {0} folder.", Setting.GetPluginFolderPath() ) );
            }
        }


        private void ClearAlerts()
        {
            SetWarningMessage( String.Empty );
            SetSuccessMessage( String.Empty );
        }

        private void ClearConnectionControl()
        {
            btnTestConnection.Visibility = Visibility.Hidden;
            icConnectionSettings.Items.Clear();
            ConnectionControl = null;
        }

        private void GetIntegrations()
        {
            var container = new IntegrationContainer( Setting.GetPluginFolderPath() );

            Integrations = container.GetIntegrations();

        }

        private bool LoadIntegrationConnectionControl( string integrationName )
        {
            bool controlLoaded = false;
            ClearConnectionControl();


            var integration = Integrations.Where( i => i.Name == integrationName ).FirstOrDefault();

            if ( integration != null )
            {
                ConnectionControl = integration.Component.GetConnectionControl();
                icConnectionSettings.Items.Add( ConnectionControl );
                controlLoaded = true;
            }

            return controlLoaded;
        }

        private void SetControlVisibility( bool isVisible )
        {
            if ( isVisible )
            {
                icConnectionSettings.Visibility = System.Windows.Visibility.Visible;
                btnTestConnection.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {

                icConnectionSettings.Visibility = Visibility.Hidden;
                btnTestConnection.Visibility = Visibility.Hidden;
            }
        }

        private void SetWarningMessage( string message )
        {
            lblWarning.Content = message;
            lblWarning.Visibility = !String.IsNullOrWhiteSpace( message ) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetSuccessMessage( string message )
        {
            lblSuccessAlert.Content = message;
            lblSuccessAlert.Visibility = !String.IsNullOrWhiteSpace( message ) ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool TestConnection()
        {
            bool isSuccess = false;
            ClearAlerts();
            var integration = Integrations.Where( i => i.Name == cboDataSource.SelectedValue.ToString() ).FirstOrDefault();

            if ( integration == null )
            {
                SetWarningMessage( "Data Source Not Found." );
                return isSuccess;
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();

            if(!ConnectionControl.IsValid(ref errors))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine( "Connection Information is not valid. Please correct the following:" );
                foreach ( var error in errors )
                {
                    sb.AppendLine( String.Format( "- {0}", error.Value ) );
                }

                SetWarningMessage( sb.ToString() );
                return false;
            }
            string errorMessage = String.Empty;
            isSuccess = integration.Component.TestConnection( ConnectionControl.Value, out errorMessage );

            if ( !isSuccess )
            {
                SetWarningMessage( errorMessage );
            }

            return isSuccess;
          
        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {
            TestConnection();
        }

    }
}
