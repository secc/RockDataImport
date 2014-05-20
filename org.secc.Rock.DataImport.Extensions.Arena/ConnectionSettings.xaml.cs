using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using org.secc.Rock.DataImport.BAL.Integration;
namespace org.secc.Rock.DataImport.Extensions.Arena
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings : IntegrationConnectionControl
    {

        public override Dictionary<string, string> Value
        {
            get
            {
                return GetFieldValues();
            }
            set
            {
                BindFields( value );
            }
        }

        public ConnectionSettings() : base()
        {
            InitializeComponent();

        }

        public override void Load()
        {
            Load( null );
        }

        public override void Load( Dictionary<string, string> settings )
        {
            Clear();



            if ( settings.Count > 0 )
            {
                Value = settings;
            }
        }

        public override void Clear()
        {
            ClearFields(); 

        }

        public override bool IsValid( ref Dictionary<string, string> errors )
        {
            if(errors == null)
            {
                errors = new Dictionary<string, string>();
            }

            if ( String.IsNullOrWhiteSpace( txtDatabaseServer.Text ) )
            {
                errors.Add( "Database Server", "Database Server is required" );
            }

            if ( String.IsNullOrWhiteSpace( txtDatabaseName.Text ) )
            {
                errors.Add( "Database Name", "Database Name is required." );
            }

            if ( chkIntegratedSecurity.IsChecked == null || !(bool)chkIntegratedSecurity.IsChecked )
            {
                if ( String.IsNullOrWhiteSpace( txtUserName.Text ) )
                {
                    errors.Add( "User Name", "Username is required if Integrated Security is not selected." );
                }

                if ( String.IsNullOrWhiteSpace( txtPassword.Password ) )
                {
                    errors.Add( "Password", "Password is required if Integrated Security is not selected." );
                }
            }

            return errors.Count == 0;
        }

        private void chkIntegratedSecurity_Checked( object sender, RoutedEventArgs e )
        {
            ToggleUserPasswordFields( (bool)chkIntegratedSecurity.IsChecked );
        }



        private void BindFields( Dictionary<string, string> settings )
        {
            ClearFields();
            if ( settings.ContainsKey( "DatabaseServer" ) )
            {
                txtDatabaseServer.Text = settings["DatabaseServer"];
            }

            if ( settings.ContainsKey( "DatabaseName" ) )
            {
                txtDatabaseName.Text = settings["DatabaseName"];
            }

            bool useIntegratedSecurity = false;

            if ( settings.ContainsKey( "IntegratedSecurity" ) )
            {
                bool.TryParse( settings[ "IntegratedSecurity" ], out useIntegratedSecurity );

                chkIntegratedSecurity.IsChecked = useIntegratedSecurity;
            }

            if ( !useIntegratedSecurity )
            {
                if ( settings.ContainsKey( "UserName" ) )
                {
                    txtUserName.Text = settings["UserName"];
                }

                if ( settings.ContainsKey( "Password" ) )
                {
                    txtPassword.Password = settings["Password"];
                }
            }
        }

        private void ClearFields()
        {
            txtDatabaseServer.Text = String.Empty;
            txtDatabaseName.Text = String.Empty;
            txtUserName.Text = String.Empty;
            txtPassword.Password = String.Empty;

            chkIntegratedSecurity.IsChecked = false;
            ToggleUserPasswordFields( false );
        }

        private Dictionary<string, string> GetFieldValues()
        {
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();

            if ( !String.IsNullOrWhiteSpace( txtDatabaseServer.Text ) )
            {
                fieldValues.Add( "DatabaseServer", txtDatabaseServer.Text.Trim() );
            }

            if ( !String.IsNullOrWhiteSpace( txtDatabaseName.Text ) )
            {
                fieldValues.Add( "DatabaseName", txtDatabaseName.Text.Trim() );
            }

            if ( chkIntegratedSecurity.IsChecked == null )
            {
                fieldValues.Add( "IntegratedSecurity", bool.FalseString );
            }
            else
            {
                fieldValues.Add( "IntegratedSecurity", ((bool)chkIntegratedSecurity.IsChecked).ToString() );
            }

            if ( chkIntegratedSecurity.IsChecked == false )
            {
                if(!String.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    fieldValues.Add( "UserName", txtUserName.Text.Trim() );                
                }

                if ( !String.IsNullOrWhiteSpace( txtPassword.Password ) )
                {
                    fieldValues.Add( "Password", txtPassword.Password.Trim() );
                }
            }

            return fieldValues;

        }


        private void ToggleUserPasswordFields( bool useIntegratedSecurity )
        {
            txtUserName.IsEnabled = !useIntegratedSecurity;
            txtPassword.IsEnabled = !useIntegratedSecurity;

            if(useIntegratedSecurity)
            {
                txtUserName.Text = String.Empty;
                txtPassword.Password = String.Empty;    
            }
        }



    }
}
