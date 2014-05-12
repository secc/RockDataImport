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
        string Url;
        string Username;
        string Password;

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
            btnLogin.IsEnabled = false;

            Url = txtRockURL.Text;
            Username = txtRockUser.Text;
            Password = txtRockPassword.Password;

            this.Cursor = Cursors.Wait;
            BackgroundWorker loginWorker = new BackgroundWorker();
            loginWorker.DoWork += loginWorker_DoWork;
            loginWorker.RunWorkerCompleted += loginWorker_RunWorkerCompleted;
            loginWorker.RunWorkerAsync();
        }

        void loginWorker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            this.Cursor = null;
            btnLogin.IsEnabled = true;

            if(e.Error != null)
            {
                throw e.Error;
            }

            if ( App.RockConnection != null && App.RockConnection.Client != null )
            {
                SaveLoginSettings();

                if(this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
                else
                {
                    this.NavigationService.Navigate(new ConfigurationPage());
                }
            }
        }

        void loginWorker_DoWork( object sender, DoWorkEventArgs e )
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
            Url = null;
            Username = null;
            Password = null;
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

        private bool Login()
        {
            try
            {
                App.RockConnection = new BAL.RockConnection();
                App.RockConnection.Connect( Url, Username, Password );

                return true;
            }
            catch (Exception wEx )
            {

                ////System.Net.HttpWebResponse response = wEx.Response as System.Net.HttpWebResponse;

                //if ( response != null )
                //{
                //    if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
                //    {
                //        lblLoginWarning.Content = "Invalid Login";
                //        lblLoginWarning.Visibility = System.Windows.Visibility.Visible;
                //    }
                //    else
                //    {
                //        StringBuilder messageBuilder = new StringBuilder();
                //        messageBuilder.Append( wEx.Message );

                //        if ( wEx.InnerException != null )
                //        {
                //            messageBuilder.AppendLine( wEx.InnerException.Message );
                //        }
                //    }
                //}

                return false;
            }
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

        #endregion

    }
}
