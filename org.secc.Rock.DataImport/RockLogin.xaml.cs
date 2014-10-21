using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using org.secc.Rock.DataImport.BAL;

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
            SetWarningMessage( String.Empty );
            LoadCachedLoginCredentials();

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
            SetWarningMessage( String.Empty );
            ClearForm();
            LoadCachedLoginCredentials();
        }



        #endregion

        #region Private
        private void ClearForm()
        {
            txtRockURL.Text = String.Empty;
            txtUsername.Text = String.Empty;
            txtPassword.Password = String.Empty;
            SetWarningMessage( String.Empty );
        }


        /// <summary>
        /// Hides the login warning.
        /// </summary>
        private void LoginControl_KeyDown( object sender, KeyEventArgs e )
        {
            SetWarningMessage(String.Empty);


            //If User pressed Enter and username and password have been populated, log the user in.
            if ( e.Key == Key.Enter )
            {
                if ( !String.IsNullOrWhiteSpace( txtUsername.Text ) && !String.IsNullOrWhiteSpace( txtPassword.Password ) )
                {
                    Login();
                }
            }
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
            txtUsername.Text = !String.IsNullOrWhiteSpace( rockUser ) ? rockUser : String.Empty;

        }

        private void Login()
        {
            SetWarningMessage(String.Empty);
            string url = txtRockURL.Text;
            string userName = txtUsername.Text;
            string password = txtPassword.Password;


            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += delegate( object s, DoWorkEventArgs ee )
            {
                ee.Result = null;
                App.RockService = new BAL.RockService( url );
                App.RockService.Login( userName, password );

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

                catch ( RockServiceException rex )
                {

                    if ( rex.StatusCode.Equals( System.Net.HttpStatusCode.Unauthorized ) )
                    {
                        SetWarningMessage( "Invalid Login" );
                        return;
                    }

                    StringBuilder messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine( rex.Message );
                    if ( rex.InnerException != null )
                    {
                        messageBuilder.AppendLine( rex.InnerException.Message );
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

            if ( !String.IsNullOrWhiteSpace( Setting.GetSettingValue( SETTING_CATEGORY, "RockUser" ) ) || Setting.GetSettingValue( SETTING_CATEGORY, "RockUser" ) != txtUsername.Text )
            {
                Setting.UpdateSettingValue( SETTING_CATEGORY, "RockUser", txtUsername.Text );
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
