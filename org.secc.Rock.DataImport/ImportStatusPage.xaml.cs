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
using System.ComponentModel;

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for ImportStatusPage.xaml
    /// </summary>
    public partial class ImportStatusPage : Page
    {
        ExportIntegrations Integration;
        BackgroundWorker ImportBackgroundWorker;
        int MaxFailureCount = 0;
        int MaxBatchSize = 0;
        int MaxRecordsToImport = 0;

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

        private void btnStop_Click( object sender, RoutedEventArgs e )
        {
            ImportBackgroundWorker.CancelAsync();
            ShowStatusMessage( "Import Stopped", false );

        }
        #endregion

        #region Private Methods

        private void BindMapGrid()
        {
            grdMaps.ItemsSource =  Integration.Component.ExportMaps
                                    .Where(m => m.Selected)
                                    .OrderByDescending( m => m.ImportRanking )
                                        .ThenBy( m => m.Name );
            grdMaps.Items.Refresh();

        }

        private void SetProgressVisibility( bool display )
        {
            if ( display )
            {
                grdContent.Visibility = System.Windows.Visibility.Visible;
                btnFinish.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                grdContent.Visibility = System.Windows.Visibility.Hidden;
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

        private void SetStopButtonVisibility( bool display )
        {
            if ( display )
            {
                btnStop.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnStop.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ShowStatusMessage( string message, bool isError )
        {
            lblImportStatus.Content = message;

            if ( !String.IsNullOrWhiteSpace( message ) )
            {
                lblImportStatus.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lblImportStatus.Visibility = System.Windows.Visibility.Hidden;
            }

            if ( isError )
            {
                lblImportStatus.Style = (Style)this.Resources["labelStyleAlertError"];
            }
            else
            {
                lblImportStatus.Style = (Style)this.Resources["lableStyleAlertInfo"];
            }
        }

        private void StartImportProcess()
        {
            ImportBackgroundWorker = new BackgroundWorker();
            ImportBackgroundWorker.WorkerSupportsCancellation = true;
            ImportBackgroundWorker.DoWork += ImportBackgroundWorker_DoWork;
            ImportBackgroundWorker.RunWorkerCompleted += ImportBackgroundWorker_RunWorkerCompleted;


            BindMapGrid();
            SetStartVisibility( false );
            SetProgressVisibility( true );
            SetStopButtonVisibility( true );
            btnBack.IsEnabled = false;
        
        }

        void ImportBackgroundWorker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            
            
            SetStopButtonVisibility( false );
            btnFinish.IsEnabled = true;
            btnBack.IsEnabled = true;

        }

        void ImportBackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
        {
            MaxBatchSize = Setting.GetImportBatchSize();

            if(MaxBatchSize == 0)
            {
                MaxBatchSize = 1000;
            }

            MaxFailureCount = Setting.GetMaxFailureCount();
            MaxRecordsToImport = Setting.GetMaxRecordsToImport();

            foreach ( var map in Integration.Component.ExportMaps.Where( x => x.Selected ).OrderByDescending( m => m.ImportRanking ).ThenBy( m => m.Name ) )
            {
                if ( ImportBackgroundWorker.CancellationPending )
                {
                    return;
                }

                ProcessImport( map );

            }
        }

        private void ProcessImport( ExportMap map )
        {
            int totalToImport = 0;
            map.Status = ExportMap.ExportStatus.Importing;
            map.Component.ExportAttemptCompleted += Component_ExportAttemptCompleted;
            bool continueImport = true;



            if ( MaxRecordsToImport == 0 || MaxRecordsToImport > map.Component.RecordCount )
            {
                totalToImport = (int)map.Component.RecordCount;
            }
            else
            {
                totalToImport = MaxRecordsToImport;
            }


            while ( continueImport )
            {
                int batchSize = 0;

                if ( ( map.Component.TotalProcessed + MaxBatchSize ) > totalToImport )
                {
                    batchSize = totalToImport - map.Component.TotalProcessed;
                }
                else
                {
                    batchSize = MaxBatchSize;
                }

                if ( !ImportBackgroundWorker.CancellationPending )
                {
                    List<string> identifiers = map.Component.GetSubsetIDs( map.Component.TotalProcessed - 1, batchSize );

                    foreach ( string identifier in identifiers )
                    {
                        if ( ImportBackgroundWorker.CancellationPending )
                        {
                            continueImport = false;
                            break;
                        }

                        map.Component.ExportRecord( identifier );

                        if ( MaxFailureCount > 0 && map.Component.FailCount > MaxFailureCount )
                        {
                            map.Status = ExportMap.ExportStatus.Failed;
                            continueImport = false;
                            break;
                        }

                    }

                    if ( map.Component.TotalProcessed == totalToImport )
                    {
                        map.Status = ExportMap.ExportStatus.Completed;
                        continueImport = false;
                    }
                }
                else
                {
                    continueImport = false;
                }
            }
        }

        void Component_ExportAttemptCompleted( object sender, ExportMapEventArgs e )
        {
            ExportMap map = Integration.Component.ExportMaps.Where( m => m.Component.GetType() == e.MapType ).FirstOrDefault();
            if ( e.IsSuccess )
            {

                map.Component.SuccessCount++;

            }
            else
            {
                map.Component.FailCount++;
            }
        }





        #endregion
    }
}
