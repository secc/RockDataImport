﻿using System;
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

        }

        private void SetProgressVisibility( bool display )
        {
            if ( display )
            {
                btnFinish.Visibility = System.Windows.Visibility.Visible;
                grdContent.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnFinish.Visibility = System.Windows.Visibility.Hidden;
                grdContent.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SetStartVisibility( bool display)
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
    }
}
