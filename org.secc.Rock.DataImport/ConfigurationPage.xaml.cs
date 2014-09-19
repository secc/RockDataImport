using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        const string SETTING_CATEGORY = "Datasource";

        public ConfigurationPage()
        {
            InitializeComponent();

        }

        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            GetIntegrations();
            BindDataSourceList();
            LoadSettings();
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
            BackgroundWorker worker = new BackgroundWorker();
            bool connectionSuccess = false;
            string warningMessage = null;
            string connectionName = cboDataSource.SelectedValue.ToString();
            Dictionary<string, string> connectionInfo = ConnectionControl.Value;

            Dictionary<string, string> errors = new Dictionary<string, string>();

            if ( !ConnectionControl.IsValid( ref errors ) )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine( "Connection Information is not valid. Please correct the following:" );
                foreach ( var error in errors )
                {
                    sb.AppendLine( String.Format( "- {0}", error.Value ) );
                }

                SetWarningMessage( sb.ToString() );
                return;
            }

            worker.DoWork += delegate( object s, DoWorkEventArgs ee )
            {
                connectionSuccess = TestConnection(connectionName, connectionInfo, out warningMessage);
            };

            worker.RunWorkerCompleted += delegate( object s, RunWorkerCompletedEventArgs ee )
            {
                if ( ee.Error != null )
                {
                    throw ee.Error;
                }

                this.Cursor = null;
                btnTestConnection.IsEnabled = true;

                if ( connectionSuccess )
                {
                    SetSuccessMessage( "Connection Successful!" );
                }
                else
                {
                    SetWarningMessage( warningMessage );
                }
            };

            this.Cursor = Cursors.Wait;
            btnTestConnection.IsEnabled = false;
            ClearAlerts();
            worker.RunWorkerAsync();

        }

        private void BindDataSourceList()
        {
            cboDataSource.Items.Clear();
            if ( Integrations.Count > 0 )
            {
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

        private void LoadSettings()
        {
            string dataSourceName = Setting.GetSettingValue( SETTING_CATEGORY, "DataSourceName" );

            if ( !String.IsNullOrWhiteSpace( dataSourceName ) && cboDataSource.Items.Contains( dataSourceName ) )
            {
                cboDataSource.SelectedValue = dataSourceName;
                LoadIntegrationConnectionControl( dataSourceName );
                SetControlVisibility( true );
            }
            else
            {
                return;
            }

            var dsSettings = Setting.GetSettingsByCategory( SETTING_CATEGORY )
                                .Where(s => s.Name != "DataSourceName");

            Dictionary<string, string> settingDictionary = new Dictionary<string, string>();

            foreach ( var setting in dsSettings )
            {
                settingDictionary.Add( setting.Name, setting.Value );
            }

            ConnectionControl.Value = settingDictionary;
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

        private void SaveConnectionSettings()
        {
            bool settingsUpdated = false;

            if ( !String.IsNullOrWhiteSpace( Setting.GetSettingValue( SETTING_CATEGORY, "DataSourceName" ) ) || Setting.GetSettingValue( SETTING_CATEGORY, "DataSourceName" ) != cboDataSource.SelectedValue.ToString() )
            {
                settingsUpdated = true;
                Setting.UpdateSettingValue( SETTING_CATEGORY, "DataSourceName", cboDataSource.SelectedValue.ToString() );
            }

            foreach ( var connectionSetting in ConnectionControl.Value )
            {
                if ( String.IsNullOrWhiteSpace( Setting.GetSettingValue( SETTING_CATEGORY, connectionSetting.Key ) ) || Setting.GetSettingValue( SETTING_CATEGORY, connectionSetting.Key ) != connectionSetting.Value )
                {
                    settingsUpdated = true;
                    Setting.UpdateSettingValue( SETTING_CATEGORY, connectionSetting.Key, connectionSetting.Value );
                }
            }

            if ( settingsUpdated )
            {
                Setting.SaveSettings();
            }

        }

        private bool TestConnection(string connectionName, Dictionary<string,string> connectionInfo, out string warningMessage)
        {
            bool isSuccess = false;
            warningMessage = null;

            var integration = Integrations.Where( i => i.Name == connectionName ).FirstOrDefault();

            if ( integration == null )
            {
                warningMessage = "Data Source Not Found.";
                return isSuccess;
            }


            isSuccess = integration.Component.TestConnection( connectionInfo, out warningMessage );


            return isSuccess;
          
        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {
            BackgroundWorker worker = new BackgroundWorker();
            bool connectionSuccess = false;
            string warningMessage = null;
            string connectionName = cboDataSource.SelectedValue.ToString();
            Dictionary<string, string> connectionInfo = ConnectionControl.Value;

            Dictionary<string, string> errors = new Dictionary<string, string>();

            if ( !ConnectionControl.IsValid( ref errors ) )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine( "Connection Information is not valid. Please correct the following:" );
                foreach ( var error in errors )
                {
                    sb.AppendLine( String.Format( "- {0}", error.Value ) );
                }

                SetWarningMessage( sb.ToString() );
                return;
            }

            worker.DoWork += delegate( object s, DoWorkEventArgs ee )
            {
                connectionSuccess = TestConnection( connectionName, connectionInfo, out warningMessage );
            };

            worker.RunWorkerCompleted += delegate( object s, RunWorkerCompletedEventArgs ee )
            {
                if ( ee.Error != null )
                {
                    throw ee.Error;
                }

                this.Cursor = null;
                btnNext.IsEnabled = true;

                if ( connectionSuccess )
                {
                    ExportIntegrations integration = Integrations.Where( x => x.Name == connectionName ).FirstOrDefault();
                    integration.Component.ConnectionInfo = ConnectionControl.Value;
                    this.NavigationService.Navigate( new SelectImportsPage( integration ) );
                }
                else
                {
                    SetWarningMessage( warningMessage );
                }
            };

            this.Cursor = Cursors.Wait;
            btnNext.IsEnabled = false;
            ClearAlerts();
            worker.RunWorkerAsync();

            //string warningMessage = null;
            //string connectionName = cboDataSource.SelectedValue.ToString();
            //Dictionary<string, string> connectionInfo = ConnectionControl.Value;
            //ClearAlerts();
            //if ( TestConnection(connectionName, connectionInfo, out warningMessage) )
            //{
            //    SaveConnectionSettings();
            //    //}
            //    ExportIntegrations integration = Integrations.Where( x => x.Name == cboDataSource.SelectedValue.ToString() ).FirstOrDefault();
            //    integration.Component.ConnectionInfo = ConnectionControl.Value;

            //    this.NavigationService.Navigate( new SelectImportsPage( integration ) );
            //}
        }



    }
}
