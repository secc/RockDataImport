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

namespace org.secc.Rock.DataImport
{
    /// <summary>
    /// Interaction logic for DefinedTypeMappingPage.xaml
    /// </summary>
    public partial class DefinedTypeMappingPage : Page
    {
        ExportIntegrations Integration { get; set; }
        List<MappedDefinedType> MappedDefinedTypes = null;
        bool isDirty = false;
        string currentFKValue;

        private DefinedTypeMappingPage()
        {
            InitializeComponent();
        }

        public DefinedTypeMappingPage( ExportIntegrations integration )
        {
            Integration = integration;
            InitializeComponent();
        }


        #region Page Events
        private void Page_Loaded( object sender, RoutedEventArgs e )
        {
            SetStatusMessage( "Loading and mapping defined types... This could take a couple moments." );
            LoadDefinedTypes();
            SetStatusMessage( String.Empty );
            BindDefinedTypeGrid();
            SetDefinedTypeDetailVisibility( false );
            SetNextButtonStatus();

        }

        private void btnBack_Click( object sender, RoutedEventArgs e )
        {
            if ( NavigationService.CanGoBack )
            {
                NavigationService.GoBack();
            }
        }

        private void btnNext_Click( object sender, RoutedEventArgs e )
        {

        }

        private void dgDataMaps_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( isDirty )
            {
                var removedDT = e.RemovedItems[0] as MappedDefinedType;
                string message = string.Format( "Do you want to save the changes to the {0} defined type.", removedDT.Name );

                var msgResponse = MessageBox.Show( message, "Save Changes", MessageBoxButton.YesNo );

                if ( msgResponse == MessageBoxResult.Yes )
                {
                    SaveDefinedValueMapping( removedDT );
                }
            }
            
            MappedDefinedType dt = (MappedDefinedType)dgDataMaps.SelectedItem;

            if ( dt != null )
            {
                LoadDefinedTypeDetail( dt );
            }
        }

        private void cboRockDefinedValue_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {

            DefinedValueSummary selectedValue = dgDataType.SelectedCells[0].Item as DefinedValueSummary;

            if ( String.IsNullOrEmpty( currentFKValue ) && !String.IsNullOrEmpty( selectedValue.ForeignId ) )
            {
                isDirty = true;
            }
            else if ( !String.IsNullOrEmpty( currentFKValue ) && selectedValue.ForeignId != currentFKValue )
            {
                isDirty = true;
            }
            else if ( !String.IsNullOrEmpty( currentFKValue ) && currentFKValue == selectedValue.ForeignId )
            {
                isDirty = false;
            }

            MappedDefinedType mdt = dgDataMaps.SelectedItem as MappedDefinedType;

            SetSaveButtonStatus( mdt.SourceDefinedTypeSummary.ValueSummaries.Where( vs => String.IsNullOrEmpty( vs.ForeignId ) ).Count() == 0 );
        }

        private void dgDataType_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
        }

        private void dgDataType_SelectedCellsChanged( object sender, SelectedCellsChangedEventArgs e )
        {

            if ( e.AddedCells.Count == 0 )
            {
                return;
            }

            var currentCell = e.AddedCells[0];

            if ( currentCell.Column == dgDataType.Columns[1] )
            {
                dgDataType.BeginEdit();
            }


            currentFKValue = ( dgDataType.SelectedCells[0].Item as DefinedValueSummary ).ForeignId;

        }

        private void btnSave_Click( object sender, RoutedEventArgs e )
        {
            SaveDefinedValueMapping((MappedDefinedType)dgDataMaps.SelectedItem);
        }

        private void btnReset_Click( object sender, RoutedEventArgs e )
        {
            ResetDefinedValueMapping((MappedDefinedType)dgDataMaps.SelectedItem);
            LoadDefinedTypeDetail( (MappedDefinedType)dgDataMaps.SelectedItem );
        }

        #endregion

        #region Private 

        private void BindDefinedTypeGrid()
        {
            if ( MappedDefinedTypes.Count == 0 )
            {
                SetStatusMessage( "No Defined Types were found to map." );
                return;
            }

            dgDataMaps.ItemsSource = MappedDefinedTypes.OrderBy( mdt => mdt.Name );


        }

        private void CreateRockDefinedType(MappedDefinedType mappedDT)
        {

            DefinedTypeMap dtMap = new DefinedTypeMap( App.RockService );
            int? rockDefinedTypeId = null;

            try
            {
                if ( mappedDT.SourceDefinedTypeSummary != null )
                {
                    rockDefinedTypeId = dtMap.Save( category: mappedDT.SourceDefinedTypeSummary.Category,
                            name: mappedDT.SourceDefinedTypeSummary.Name,
                            description: mappedDT.SourceDefinedTypeSummary.Description,
                            isSystem: mappedDT.SourceDefinedTypeSummary.IsSystem,
                            order: mappedDT.SourceDefinedTypeSummary.Order,
                            foreignId: mappedDT.SourceDefinedTypeSummary.Id,
                            helpText: mappedDT.SourceDefinedTypeSummary.HelpText );
                }
                else
                {
                    throw new Exception( "Source Defined Type is null." );
                }
            }
            catch ( Exception ex )
            {

                throw new DefinedTypeSaveException( "An error has occurred while creating a Rock Defined Type", ex, mappedDT.SourceId, mappedDT.RockGuid );
            }

            foreach ( DefinedValueSummary dvs in mappedDT.SourceDefinedTypeSummary.ValueSummaries )
            {
                CreateRockDefinedValue( dvs, (int)rockDefinedTypeId );
            }


            mappedDT.RockDefinedTypeSummary = dtMap.GetDefinedTypeSummary( (int)rockDefinedTypeId );
            mappedDT.RockGuid = mappedDT.RockDefinedTypeSummary.UniqueIdentifier.ToString();
            mappedDT.SourceDefinedTypeSummary.ForeignId = mappedDT.RockDefinedTypeSummary.Id.ToString();
           
        }

        private void CreateRockDefinedValue( DefinedValueSummary dvSummary, int rockDefindTypeId )
        {
            try
            {
                DefinedValueMap dvMap = new DefinedValueMap( App.RockService );
                int? dvId =  dvMap.Save( definedTypeId: rockDefindTypeId, value: dvSummary.Value, description: dvSummary.Description, isSystem: dvSummary.IsSystem, order: dvSummary.Order, foreignId: dvSummary.Id.ToString() );

                if ( dvId == null )
                {
                    throw new Exception( "Defined Value not saved." );
                }
            }
            catch ( Exception ex )
            {
                throw new DefinedValueSaveException( "An error occurred while creating a Rock defined value.", ex, dvSummary.Id, null, rockDefindTypeId );
            }

        }


        private DefinedTypeSummary GetRockDefinedType( string rockGuidString )
        {
            DefinedTypeMap dtm = new DefinedTypeMap( App.RockService );
            DefinedTypeSummary summary = null;

            Guid rockGuid;

            if ( Guid.TryParse( rockGuidString, out rockGuid ) )
            {
                summary = dtm.GetDefinedTypeSummary( rockGuid );
            }

            return summary;
        }

        private DefinedTypeSummary GetRockDefinedTypeByForeignKey( string sourceForeignId )
        {
            DefinedTypeMap dtm = new DefinedTypeMap( App.RockService );

            return dtm.GetDefinedTypeSummaryByForeignId( sourceForeignId );

        }

        private DefinedTypeSummary GetSourceDefinedType( string sourceId )
        {
            return Integration.Component.GetDefinedTypeSummary( sourceId );
        }

        /// <summary>
        /// Loads the defined types that are referenced in the defined type attributes for the selected maps into the MappedDefinedTypes list.
        /// </summary>
        private void LoadDefinedTypes()
        {
            MappedDefinedTypes = new List<MappedDefinedType>();
            foreach ( var map in Integration.Component.ExportMaps.Where(m => m.Selected ) )
            {
                LoadDefinedTypes( map.Component );
            }

            foreach ( var mappedDT in MappedDefinedTypes )
            {
                mappedDT.SourceDefinedTypeSummary = GetSourceDefinedType( mappedDT.SourceId );

                if(mappedDT.SourceDefinedTypeSummary == null)
                {
                    throw new Exception( string.Format( "Unable to load defined type from {0} (source). Source defined value id = {1}", Integration.Name, mappedDT.SourceId ) );
                }

                if(!String.IsNullOrWhiteSpace(mappedDT.RockGuid))
                {
                    mappedDT.RockDefinedTypeSummary = GetRockDefinedType( mappedDT.RockGuid );

                }

                if ( mappedDT.RockDefinedTypeSummary == null )
                {
                    mappedDT.RockDefinedTypeSummary = GetRockDefinedTypeByForeignKey( mappedDT.SourceDefinedTypeSummary.Id );

                    if ( mappedDT.RockDefinedTypeSummary == null )
                    {
                        mappedDT.RockDefinedTypeSummary = new DefinedTypeSummary();
                        mappedDT.RockDefinedTypeSummary.IsSystem = mappedDT.SourceDefinedTypeSummary.IsSystem;
                        mappedDT.RockDefinedTypeSummary.Order = mappedDT.SourceDefinedTypeSummary.Order;
                        mappedDT.RockDefinedTypeSummary.Category = mappedDT.SourceDefinedTypeSummary.Category;
                        mappedDT.RockDefinedTypeSummary.Name = mappedDT.SourceDefinedTypeSummary.Name;
                        mappedDT.RockDefinedTypeSummary.Description = mappedDT.SourceDefinedTypeSummary.Description;
                        mappedDT.RockDefinedTypeSummary.ForeignId = mappedDT.SourceDefinedTypeSummary.ForeignId;
                        mappedDT.RockDefinedTypeSummary.HelpText = mappedDT.SourceDefinedTypeSummary.HelpText;

                        SaveRockDefinedType( mappedDT.RockDefinedTypeSummary );
                        mappedDT.RockGuid = mappedDT.RockDefinedTypeSummary.UniqueIdentifier.ToString();

                        foreach ( var valueSummary in mappedDT.SourceDefinedTypeSummary.ValueSummaries )
                        {
                            DefinedValueSummary dvs = new DefinedValueSummary();
                            dvs.IsSystem = valueSummary.IsSystem;
                            dvs.DefinedTypeId = mappedDT.RockDefinedTypeSummary.Id;
                            dvs.Order = valueSummary.Order;
                            dvs.Value = valueSummary.Value;
                            dvs.Description = valueSummary.Description;
                            dvs.ForeignId = valueSummary.ForeignId;

                            SaveRockDefinedValue( dvs );

                            if ( mappedDT.RockDefinedTypeSummary.ValueSummaries == null )
                            {
                                mappedDT.RockDefinedTypeSummary.ValueSummaries = new List<DefinedValueSummary>();
                            }

                            mappedDT.RockDefinedTypeSummary.ValueSummaries.Add( dvs );

                        }

                    }
                }
                else
                {
                    if(String.IsNullOrEmpty(mappedDT.RockDefinedTypeSummary.ForeignId))
                    {
                        mappedDT.RockDefinedTypeSummary.ForeignId = mappedDT.SourceDefinedTypeSummary.Id;
                        SaveRockDefinedType( mappedDT.RockDefinedTypeSummary );
                    }

                }

                SetSourceDefinedValueForeignIds( mappedDT );

            }
        }

        private void LoadDefinedTypeDetail(MappedDefinedType dt)
        {
            if ( dt.RockDefinedTypeSummary == null )
            {
                return;
            }
            ResetDefinedTypeDetail();

            lblDefindTypeName.Content = dt.RockDefinedTypeSummary.Name;
            tbDefinedTypeDescription.Text = dt.RockDefinedTypeSummary.Description;

            var rockValueViewSource = ( (CollectionViewSource)( FindResource( "rockValueSummaries" ) ) );
            rockValueViewSource.Source = dt.RockDefinedTypeSummary.ValueSummaries.OrderBy( vs => vs.Order ).ToList();
            dgDataType.ItemsSource = dt.SourceDefinedTypeSummary.ValueSummaries.OrderBy(vs => vs.Order).ToList();
            SetDefinedTypeDetailVisibility( true );
        }


        /// <summary>
        /// Loads the distinct defined types for the export map and the maps that it is dependent on 
        /// </summary>
        /// <param name="mapComponent">The export map.</param>
        private void LoadDefinedTypes( iExportMapComponent mapComponent )
        {

            foreach ( var attribute in mapComponent.GetAttributes(typeof(DefinedTypeAttribute)) )
            {
                if ( MappedDefinedTypes.Where( dt => dt.SourceId == attribute.Value["SourceDefinedTypeIdentifier"].ToString() )
                        .Where( dt => dt.RockGuid == attribute.Value["RockDefinedTypeGuid"].ToString() )
                        .Count() == 0 )
                {
                    MappedDefinedTypes.Add( new MappedDefinedType()
                            {
                                Name = attribute.Key,
                                SourceId = attribute.Value["SourceDefinedTypeIdentifier"].ToString(),
                                RockGuid = attribute.Value["RockDefinedTypeGuid"].ToString(),
                                SourceDefinedTypeSummary = null,
                                RockDefinedTypeSummary = null
                            } );
                }
            }

            var dependantMaps = Integration.Component.ExportMaps
                                    .Where( m => !m.Selected )
                                    .Where( m => mapComponent.GetAttributes( typeof( DependencyAttribute ) ).Select( da => ( (Type)da.Value["Dependency"] ).ToString() ).Contains( m.Component.GetType().ToString() ) )
                                    .Select( m => m.Component );

            foreach ( var map in dependantMaps )
            {
                LoadDefinedTypes( map );
            }
        }

        private void RefreshDefinedTypeMapGrid()
        {
            dgDataMaps.Items.Refresh();
        }

        private void RefreshDefinedValueGrid()
        {
            dgDataType.Items.Refresh();
        }

        private void ResetDefinedTypeDetail()
        {
            isDirty = false;
            lblDefindTypeName.Content = null;
            tbDefinedTypeDescription.Text = null;
            dgDataType.ItemsSource = null;
            var rockValueViewSource = ( (CollectionViewSource)( FindResource( "rockValueSummaries" ) ) );
            rockValueViewSource.Source = null;
            currentFKValue = null;
            SetSaveButtonStatus( false );
        }

        private void ResetDefinedValueMapping( MappedDefinedType mdt )
        {
            SetSourceDefinedValueForeignIds( mdt );
            RefreshDefinedValueGrid();
            isDirty = false;
        }

        private void SaveRockDefinedType( DefinedTypeSummary dts )
        {
            try
            {
                DefinedTypeMap dtMap = new DefinedTypeMap( App.RockService );
                int? returnedDTId = dtMap.Save(
                        category: dts.Category,
                        name: dts.Name,
                        description: dts.Description,
                        isSystem: dts.IsSystem,
                        order: dts.Order,
                        foreignId: dts.ForeignId,
                        definedTypeId: int.Parse( dts.Id ),
                        helpText: dts.HelpText,
                        fieldTypeId: dts.FieldTypeId
                        );

                if ( returnedDTId == null )
                {
                    throw new Exception( "Unable to save Rock Defined Type." );
                }

                dts = dtMap.GetDefinedTypeSummary( (int)returnedDTId );

            }
            catch ( Exception ex )
            {
                throw new DefinedTypeSaveException( "An error occurred while saving Rock Defined Type.", ex,
                    MappedDefinedTypes.Where( mdt => mdt.RockDefinedTypeSummary.Id == dts.Id ).Select( mdt => mdt.SourceId ).FirstOrDefault(),
                    MappedDefinedTypes.Where( mdt => mdt.RockDefinedTypeSummary.Id == dts.Id ).Select( mdt => mdt.RockGuid ).FirstOrDefault() );
            }
        }

        private void SaveRockDefinedValue( DefinedValueSummary dvs )
        {
            try
            {
                DefinedValueMap dvMap = new DefinedValueMap( App.RockService );
                int? returnedDVId = dvMap.Save(
                    definedTypeId: int.Parse( dvs.DefinedTypeId ),
                    value: dvs.Value,
                    description: dvs.Description,
                    isSystem: dvs.IsSystem,
                    order: dvs.Order,
                    foreignId: dvs.ForeignId,
                    definedValueId: !String.IsNullOrEmpty(dvs.Id) ? int.Parse(dvs.Id) : 0
                    );

                if ( returnedDVId == null )
                {
                    throw new Exception( "Unable to save Rock Defined Value." );
                }
                else
                {
                    dvs = dvMap.GetDefinedValueSummaryById( (int) returnedDVId );
                }
            }
            catch ( Exception ex )
            {
                var mappedDV = MappedDefinedTypes.Where( x => x.RockDefinedTypeSummary.Id == dvs.DefinedTypeId ).FirstOrDefault();

                throw new DefinedValueSaveException( "An error occurred while saving a Rock defined value.", ex, mappedDV.SourceId, mappedDV.RockGuid, int.Parse( dvs.DefinedTypeId ) );
            }
        }

        private void SaveDefinedValueMapping( MappedDefinedType mdt )
        {
            foreach ( DefinedValueSummary dvs in mdt.SourceDefinedTypeSummary.ValueSummaries )
            {
                DefinedValueSummary rockDefinedValueSummary = null;
                if ( dvs.ForeignId == "-1" )
                {
                    rockDefinedValueSummary = new DefinedValueSummary();
                    rockDefinedValueSummary.DefinedTypeId = mdt.RockDefinedTypeSummary.Id;
                    rockDefinedValueSummary.Value = dvs.Value;
                    rockDefinedValueSummary.Description = dvs.Description;
                    rockDefinedValueSummary.ForeignId = dvs.Id;
                    rockDefinedValueSummary.Order = dvs.Order;
                    rockDefinedValueSummary.IsSystem = dvs.IsSystem;
                    mdt.RockDefinedTypeSummary.ValueSummaries.Add( rockDefinedValueSummary );
                }
                else
                {
                    rockDefinedValueSummary = mdt.RockDefinedTypeSummary.ValueSummaries.Where( rdvs => rdvs.Id == dvs.ForeignId ).FirstOrDefault();

                    if ( rockDefinedValueSummary.ForeignId == dvs.Id )
                    {
                        continue;
                    }

                    rockDefinedValueSummary.ForeignId = dvs.Id;
                }

                SaveRockDefinedValue( rockDefinedValueSummary );


            }

            SetSourceDefinedValueForeignIds( mdt );
            RefreshDefinedTypeMapGrid();
            RefreshDefinedValueGrid();
        }


        private void SetDefinedTypeDetailVisibility( bool showDetail )
        {
            if ( showDetail )
            {
                spDefinedTypeDetail.Visibility = System.Windows.Visibility.Visible;
                spDefinedTypeDetailInstructions.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                spDefinedTypeDetail.Visibility = System.Windows.Visibility.Collapsed;
                spDefinedTypeDetailInstructions.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void SetNextButtonStatus()
        {
            btnNext.IsEnabled = MappedDefinedTypes.Where( mdt => !mdt.IsMapped ).Count() == 0;
        }

        private void SetSaveButtonStatus( bool isEnabled )
        {
            btnSave.IsEnabled = isEnabled;
        }

        private void SetSourceDefinedValueForeignIds( MappedDefinedType mdt )
        {
            foreach ( DefinedValueSummary dvs in mdt.SourceDefinedTypeSummary.ValueSummaries )
            {
                dvs.ForeignId = mdt.RockDefinedTypeSummary.ValueSummaries
                                .Where( rdvs => rdvs.ForeignId == dvs.Id )
                                .Select( rdvs => rdvs.Id )
                                .FirstOrDefault();
            }
        }

        private void SetStatusMessage( string messageText )
        {
            if ( !String.IsNullOrWhiteSpace( messageText ) )
            {
                lblStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                lblStatus.Visibility = System.Windows.Visibility.Visible;
            }

            tbStatusText.Text = messageText;
        }



        #endregion




    }
    public  class MappedDefinedType
    {
        public string Name { get; set; }
        public string SourceId { get; set; }
        public string RockGuid { get; set; }
        public DefinedTypeSummary SourceDefinedTypeSummary { get; set; }
        public DefinedTypeSummary RockDefinedTypeSummary { get; set; }

        public bool IsMapped
        {
            get
            {
                return ValueIsMapped();
            }
        }

        public string IsMappedString
        {
            get
            {
                string value;
                if ( IsMapped )
                {
                    value = "Yes";
                }
                else
                {
                    value = "No";
                }

                return value;
            }
        }


        public bool ValueIsMapped()
        {
            bool mapped = false;

            if ( SourceDefinedTypeSummary != null && SourceDefinedTypeSummary.ValueSummaries != null )
            {
                int valuesCount = SourceDefinedTypeSummary.ValueSummaries.Count;

                if ( valuesCount > 0 && SourceDefinedTypeSummary.ValueSummaries.Where( vs => !String.IsNullOrWhiteSpace( vs.ForeignId ) ).Count() == valuesCount )
                {
                    mapped = true;
                }
            }

            return mapped;
        }

    }

    [Serializable]
    public class DefinedTypeSaveException : Exception
    {
        public string SourceIdentifier { get; set; }
        public string RockIdentifier { get; set; }



        public DefinedTypeSaveException() { }
        public DefinedTypeSaveException( string message ) : base( message ) { }
        public DefinedTypeSaveException( string message, Exception inner ) : base( message, inner ) { }

        public DefinedTypeSaveException( string message, Exception inner, string sourceIdentifier, string rockIdentifier )
            : base( message, inner )
        {
            SourceIdentifier = sourceIdentifier;
            RockIdentifier = rockIdentifier;
        }

        protected DefinedTypeSaveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }

    [Serializable]
    public class DefinedValueSaveException : Exception
    {
        public string SourceIdentifier { get; set; }
        public string RockIdentifier { get; set; }
        public int RockDefinedTypeId { get; set; }

        public DefinedValueSaveException() { }
        public DefinedValueSaveException( string message ) : base( message ) { }
        public DefinedValueSaveException( string message, Exception inner ) : base( message, inner ) { }
        public DefinedValueSaveException( string message, Exception inner, string sourceIdentifier, string rockIdentifier, int rockDefinedTypeId ) : base(message, inner) 
        {
            SourceIdentifier = sourceIdentifier;
            RockIdentifier = rockIdentifier;
            RockDefinedTypeId = RockDefinedTypeId;
        }

        protected DefinedValueSaveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }

}
