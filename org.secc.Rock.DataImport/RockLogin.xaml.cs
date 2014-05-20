using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for RockLogin.xaml
    /// </summary>
    public partial class RockLogin : Page
    {

        const string SETTING_CATEGORY = "Rock Login";

        #region Constructor
        public RockLogin() 
        {
            InitializeComponent();   
        }

        #endregion

        #region Event Handlers
        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            HideLoginWarning( null, null );
            LoadCachedLoginCredentials();

        }

        /// <summary>
        /// Hides the login warning.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HideLoginWarning( object sender, System.Windows.Input.KeyEventArgs e )
        {
            lblLoginWarning.Visibility = Visibility.Hidden;

        }
        
        /// <summary>
        /// Handles the Click event of the btnLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnLogin_Click( object sender, RoutedEventArgs e )
        {
            Login();
        }

        /// <summary>
        /// Handles the Click event of the btnReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnReset_Click( object sender, RoutedEventArgs e )
        {
            HideLoginWarning( null, null );
            ClearForm();
            LoadCachedLoginCredentials();
        }

        #endregion

        #region Private
        private void ClearForm()
        {
            txtRockURL.Text = String.Empty;
            txtRockUser.Text = String.Empty;
            txtRockPassword.Password = String.Empty;
            SetWarningMessage( String.Empty );
        }


        /// <summary>
        /// Loads the cached login credentials from settings.
        /// </summary>
        private void LoadCachedLoginCredentials()
        {
            List<Setting> loginSettings = Setting.GetSettingsByCategory( SETTING_CATEGORY );

            var rockUrl = loginSettings.Where( s => s.Name == "RockUrl" ).Select( s => s.Value ).FirstOrDefault();
            var rockUser = loginSettings.Where( s => s.Name == "RockUser" ).Select( s => s.Value ).FirstOrDefault();

            txtRockURL.Text = !String.IsNullOrWhiteSpace( rockUrl ) ? rockUrl : String.Empty;
            txtRockUser.Text = !String.IsNullOrWhiteSpace( rockUser ) ? rockUser : String.Empty;

        }

        private void Login()
        {
            SetWarningMessage(String.Empty);
            string url = txtRockURL.Text;
            string userName = txtRockUser.Text;
            string password = txtRockPassword.Password;

            App.RockConnection = new BAL.RockConnection();

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += delegate( object s, DoWorkEventArgs ee )
            {
                ee.Result = null;
                App.RockConnection.Connect( url, userName, password );

            };

            worker.RunWorkerCompleted += delegate( object s, RunWorkerCompletedEventArgs ee )
            {
                this.Cursor = null;
                btnLogin.IsEnabled = true;

                try
                {
                    if ( ee.Error != null )
                    {
                        throw ee.Error;
                    }

                    SaveLoginSettings();

                    if ( this.NavigationService.CanGoBack )
                    {
                        this.NavigationService.GoBack();
                    }
                    else
                    {
                        this.NavigationService.Navigate( new ConfigurationPage() );
                    }
                }

                catch ( System.Net.WebException wEx )
                {
                    System.Net.HttpWebResponse response = wEx.Response as System.Net.HttpWebResponse;

                    if ( response != null )
                    {
                        if ( response.StatusCode.Equals( System.Net.HttpStatusCode.Unauthorized ) )
                        {
                            SetWarningMessage( "Invalid Login" );
                            return;
                        }
                    }

                    StringBuilder messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine( wEx.Message );
                    if ( wEx.InnerException != null )
                    {
                        messageBuilder.AppendLine( wEx.InnerException.Message );
                    }

                    SetWarningMessage( messageBuilder.ToString() );
                    return;

                }
            };

            this.Cursor = Cursors.Wait;
            btnLogin.IsEnabled = false;
            worker.RunWorkerAsync();
        }

        private void SaveLoginSettings()
        {
            bool settingUpdated = false;
           
            if ( String.IsNullOrWhiteSpace( Setting.GetSettingValue( SETTING_CATEGORY, "RockUrl" ) ) || Setting.GetSettingValue( SETTING_CATEGORY, "RockUrl" ) != txtRockURL.Text )
            {
                Setting.UpdateSettingValue( SETTING_CATEGORY, "RockUrl", txtRockURL.Text );
                settingUpdated = true;
            }

            if ( !String.IsNullOrWhiteSpace( Setting.GetSettingValue( SETTING_CATEGORY, "RockUser" ) ) || Setting.GetSettingValue( SETTING_CATEGORY, "RockUser" ) != txtRockUser.Text )
            {
                Setting.UpdateSettingValue( SETTING_CATEGORY, "RockUser", txtRockUser.Text );
                settingUpdated = true;
            }

            if ( settingUpdated )
            {
                Setting.SaveSettings();
            }

        }

        private void SetWarningMessage( string message )
        {
            lblLoginWarning.Content = message;

            lblLoginWarning.Visibility = String.IsNullOrWhiteSpace( message ) ? Visibility.Hidden : Visibility.Visible;
        }

        #endregion

    }
}
