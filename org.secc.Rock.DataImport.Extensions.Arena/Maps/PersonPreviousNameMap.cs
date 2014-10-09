using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Helper;
using org.secc.Rock.DataImport.BAL.Integration;
using RockMaps = org.secc.Rock.DataImport.BAL.RockMaps;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Person Previous Name")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "People last name records for ArenaChMS. This requires that the Person records to have previously been loaded.")]
    [Dependency("Person", typeof(PersonMap), 0)]

    public class PersonPreviousNameMap : ArenaMapBase
    {

        #region Fields
        int PreviousNameAttributeId;
        #endregion

        #region Properties

        public override int? RecordCount
        {
            get { return GetRecordCount(); }
        }

        #endregion

        #region Constructors

        private PersonPreviousNameMap() : base( typeof( PersonPreviousNameMap ) ) { }

        [ImportingConstructor]
        public PersonPreviousNameMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo, [Import( "RockService" )] RockService service ) : base( typeof( PersonMap ), connectionInfo, service ) { }

        #endregion

        #region Public Methods


        public override List<string> GetSubsetIDs( int startingRecord, int size )
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                var qry = Context.PersonPreviousName
                            .OrderBy( p => p.person_id )
                            .Select( p => p.person_id.ToString() )
                            .Distinct();

                if ( startingRecord > 0 )
                {
                    qry = qry.Skip( startingRecord - 1 );
                }

                if ( size > 0 )
                {
                    qry = qry.Take( size );
                }

                return qry.ToList();
                        
            }
        }

        public override void ExportRecord( string identifier )
        {
            int previousNameAttributeId = GetPreviousNameAttributeId();
            int arenaPersonId;

            if( !int.TryParse( identifier, out arenaPersonId ) )
            {
                OnExportAttemptCompleted( identifier, false, mapType: this.GetType() );
                return;
            }

            var rockPerson = GetRockPersonByForeignId( arenaPersonId );

            if ( rockPerson == null )
            {
                OnExportAttemptCompleted( identifier, true );
                return;
            }

            int rockPersonId = (int)rockPerson["Id"];

            List<string> previousLastNames = GetArenaPreviousLastNames( arenaPersonId );
            string delimitedPreviousNames = String.Empty;

            if ( previousLastNames != null )
            {
                delimitedPreviousNames = string.Join( "|", previousLastNames );
            }

            RockMaps.AttributeMap attributeMap = new RockMaps.AttributeMap( Service );
            var attributeValue = attributeMap.GetPersonAttributeValue( previousNameAttributeId, rockPersonId );

            int? attributeValueId = null;

            if ( !String.IsNullOrWhiteSpace( delimitedPreviousNames ) )
            {
                attributeValueId = attributeMap.SaveAttributeValue(
                                    attributeId: previousNameAttributeId,
                                    value: delimitedPreviousNames,
                                    entityId: rockPersonId,
                                    isSystem: false,
                                    foreignId: null,
                                    attributeValueId:
                                    attributeValue == null ? null : (int?)attributeValue["Id"] );
            }

            OnExportAttemptCompleted( identifier, true, attributeValueId, this.GetType() );

        }

        public override Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType )
        {
            return GetAttributes( this.GetType(), attributeType );
        }
        #endregion


        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null, Type mapType = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            if ( mapType == null )
            {
                args.MapType = this.GetType();
            }
            else
            {
                args.MapType = mapType;
            }

            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if ( handler != null )
            {
                handler( this, args );
            }
        }

        public override event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        #region Private Methods

        private int? GetRecordCount()
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.PersonPreviousName.Count();
            }
        }

        private int CreatePreviousNameAttribute(Guid attributeGuid)
        {
            Guid additionalInfoCategoryGuid = new Guid( "9BB6ECEE-C34C-4C3F-9EB1-48E4093024E2" );

            RockMaps.CategoryMap categoryMap = new RockMaps.CategoryMap( Service );
            Dictionary<string, object> category = categoryMap.GetByGuid( additionalInfoCategoryGuid );
            int categoryId;

            if ( category == null )
            {
                categoryId = (int)category["Id"];
            }
            else
            {
                categoryId = categoryMap.SavePersonAttributeCategory( name: "Additional Info", description: null, parentCategoryId: null, isSystem: false,
                        iconCssClass: "fa fa-info", order: null, guid: additionalInfoCategoryGuid, categoryId: null );
            }


            RockMaps.AttributeMap attributeMap = new RockMaps.AttributeMap( Service );
            //int attributeId = attributeMap.SavePersonAttribute()

            return PreviousNameAttributeId;
        }

        private List<string> GetArenaPreviousLastNames( int personId )
        {
            using ( Model.ArenaContext context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.PersonPreviousName
                        .Where( p => p.person_id == personId )
                        .Select( p => p.last_name ).ToList();
            }

        }

        private int GetPreviousNameAttributeId()
        {
            Guid previousNameAttributeGuid = new Guid( "0D052827-D89E-4D29-AB61-94BBF9E34973" );
            
            if ( PreviousNameAttributeId > 0 )
            {
                return PreviousNameAttributeId;
            }

            RockMaps.AttributeMap attributeMap = new RockMaps.AttributeMap( Service );
            Dictionary<string, object> previousNameAttribute = attributeMap.GetByGuid( previousNameAttributeGuid );

            if ( previousNameAttribute != null )
            {
                PreviousNameAttributeId = (int)previousNameAttribute["Id"];
                return PreviousNameAttributeId;
            }

            PreviousNameAttributeId = CreatePreviousNameAttribute(previousNameAttributeGuid);
            return PreviousNameAttributeId;
        }

        private Dictionary<string, object> GetRockPersonByForeignId( int foreignId )
        {
            RockMaps.PersonMap rockPersonMap = new RockMaps.PersonMap( Service );
            return rockPersonMap.GetByForeignId( foreignId.ToString() );
        }

        #endregion
    }
}
