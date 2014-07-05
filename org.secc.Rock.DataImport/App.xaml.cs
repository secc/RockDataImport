using System;
using System.Collections.Generic;
using System.Windows;

using org.secc.Rock.DataImport.BAL;

using Rock.Wpf;
namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static List<Setting> ApplicaitonSettings = null;
        public static  RockService RockService = null;


        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            Setting.LoadSettings();
            
        }

        void App_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        {
            ErrorMessageWindow errorMessageWindow = new ErrorMessageWindow( e.Exception );
            errorMessageWindow.ShowDialog();
            e.Handled = true;
        }
    }
}
