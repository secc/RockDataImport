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
        List<ExportMap> SelectedMaps;
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
        }

        #endregion

        #region Private Methods

        private void BindMapGrid()
        {
            SelectedMaps = Integration.Component.ExportMaps
                                    .Where(m => m.Selected)
                                    .OrderByDescending( m => m.ImportRanking )
                                        .ThenBy( m => m.Name ).ToList();
            grdMaps.ItemsSource = SelectedMaps;
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

        private void ShowStatusMessage( string message )
        {
            lblImportStatus.Content = message;

            if ( !String.IsNullOrWhiteSpace( message ) )
            {
                lblImportStatus.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lblImportStatus.Visibility = System.Windows.Visibility.Collapsed;
            }


        }

        private void StartImportProcess()
        {
            ImportBackgroundWorker = new BackgroundWorker();
            ImportBackgroundWorker.WorkerReportsProgress = true;
            ImportBackgroundWorker.WorkerSupportsCancellation = true;
            ImportBackgroundWorker.DoWork += ImportBackgroundWorker_DoWork;
            ImportBackgroundWorker.ProgressChanged += ImportBackgroundWorker_ProgressChanged;
            ImportBackgroundWorker.RunWorkerCompleted += ImportBackgroundWorker_RunWorkerCompleted;

            ShowStatusMessage( String.Empty );
            BindMapGrid();
            SetStartVisibility( false );
            SetProgressVisibility( true );
            SetStopButtonVisibility( true );
            btnBack.IsEnabled = false;
            this.Cursor = Cursors.AppStarting;
            ImportBackgroundWorker.RunWorkerAsync();
        }

        private void ImportBackgroundWorker_ProgressChanged( object sender, ProgressChangedEventArgs e )
        {
            grdMaps.Items.Refresh();
        }

        void ImportBackgroundWorker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {

            this.Cursor = null;
            if ( e.Error != null )
            {
                throw e.Error;
            }

            if ( e.Cancelled )
            {
                ShowStatusMessage( "Import was stopped successfully.");
            }
            else
            {
                ShowStatusMessage("Import completed.");
            }

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

            foreach ( var map in SelectedMaps )
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
                    int startingRecord = 0;

                    if ( map.Component.TotalProcessed > 0 )
                    {
                        startingRecord = map.Component.TotalProcessed - 1;
                    }
                    List<string> identifiers = map.Component.GetSubsetIDs( startingRecord, batchSize );

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

                        var totalProcessed = SelectedMaps.Select( m => m.Component.TotalProcessed ).Sum();
                        var totalRecords = SelectedMaps.Select( m => m.Component.RecordCount ).Sum();

                        ImportBackgroundWorker.ReportProgress( ( totalProcessed / (int)totalRecords ) * 100 );
                        

                    }

                    if ( map.Component.TotalProcessed == totalToImport )
                    {
                        map.Status = ExportMap.ExportStatus.Completed;
                        continueImport = false;
                    }

                    ImportBackgroundWorker.ReportProgress( ( SelectedMaps.Select( m => m.Component.TotalProcessed ).Sum() / (int)SelectedMaps.Select( m => m.Component.RecordCount ).Sum() ) * 100 );
                }
                else
                {
                    continueImport = false;
                }
            }
        }

        void Component_ExportAttemptCompleted( object sender, ExportMapEventArgs e )
        {
            ExportMap map = SelectedMaps.Where( m => m.Component.GetType() == e.MapType ).FirstOrDefault();
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
