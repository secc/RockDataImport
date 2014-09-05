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
            LoadDefinedTypes();
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

        #endregion

        #region Private 

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

        private void SaveRockDefinedType(DefinedTypeSummary dts)
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

                dts = dtMap.GetDefinedTypeSummary( (int) returnedDTId );

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
                    definedValueId: int.Parse( dvs.Id )
                    );

                if ( returnedDVId == null )
                {
                    throw new Exception( "Unable to save Rock Defined Value." );
                }
            }
            catch ( Exception ex )
            {
                var mappedDV = MappedDefinedTypes.Where( x => x.RockDefinedTypeSummary.Id == dvs.DefinedTypeId ).FirstOrDefault();

                throw new DefinedValueSaveException( "An error occurred while saving a Rock defined value.", ex, mappedDV.SourceId, mappedDV.RockGuid, int.Parse( dvs.DefinedTypeId ) );
            }
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
                    else
                    {
                        mappedDT.RockDefinedTypeSummary.ForeignId = mappedDT.SourceDefinedTypeSummary.Id;
                        SaveRockDefinedType( mappedDT.RockDefinedTypeSummary );

                    }
                }

            }
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



        #endregion
    }
    internal class MappedDefinedType
    {
        internal string Name { get; set; }
        internal string SourceId { get; set; }
        internal string RockGuid { get; set; }
        internal DefinedTypeSummary SourceDefinedTypeSummary { get; set; }
        internal DefinedTypeSummary RockDefinedTypeSummary { get; set; }

        internal bool IsMapped
        {
            get
            {
                return ValueIsMapped();
            }
        }


        private bool ValueIsMapped()
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
