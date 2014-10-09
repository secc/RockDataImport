using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class AttributeMap : MapBase
    {
        RockService Service;

        #region Constructor
        private AttributeMap() : base() { }

        public AttributeMap( RockService service )
        {
            Service = service;
        }
        #endregion

        #region Public
        public Dictionary<string, object> GetByGuid( Guid guid )
        {
            AttributeController controller = new AttributeController( Service );

            return ToDictionary( controller.GetByGuid( guid ) );
        }

        public Dictionary<string, object> GetAttributeValue( int attributeId, int? entityId )
        {
            AttributeValuesController controller = new AttributeValuesController( Service );

            StringBuilder expressionBuilder = new StringBuilder();
            expressionBuilder.AppendFormat( "AttributeId eq {0}", attributeId );

            if ( entityId == null )
            {
                expressionBuilder.Append( " and EntityId eq null" );
            }

            AttributeValue value = controller.GetByFilter( expressionBuilder.ToString() ).FirstOrDefault();

            return ToDictionary( value );
        }

        public Dictionary<string, object> GetPersonAttributeValue( int attributeId, int personId )
        {
            return GetAttributeValue( attributeId, personId );
        }

        public int SavePersonAttribute( int fieldTypeId, string key, string name, string description = null, int order = 0, bool isGridColumn = false,
                string defaultValue = null, bool isMultiValue = false, string foreignId = null, string iconCssClass = null,
                bool isSystem = false, Guid guid = default(Guid), int? categoryId = null, int? attributeId = null )
        {
            EntityTypeController controller = new EntityTypeController( Service );
            int entityTypeId = controller.GetByFriendlyName( typeof( Person ).ToString() ).Id;

            return SaveAttribute
                    (
                        fieldTypeId: fieldTypeId,
                        key: key,
                        name: name,
                        description: description,
                        isSystem: isSystem,
                        entityTypeId: entityTypeId,
                        order: order,
                        isGridColumn: isGridColumn,
                        defaultValue: defaultValue,
                        isMultiValue: isMultiValue,
                        isRequired: false,
                        guid: guid,
                        foreignId: foreignId,
                        iconCssClass: iconCssClass,
                        categoryId: categoryId,
                        attributeId: attributeId
                    );

        }

        public int SaveAttribute(int fieldTypeId, string key, string name, bool isSystem = false, int? entityTypeId = null, string entityTypeQualifierColumn = null, 
                string entityTypeQualifierValue = null, string description = null, int order = 0, bool isGridColumn = false, string defaultValue = null,
                bool isMultiValue = false, bool isRequired = false, Guid guid = default(Guid), string foreignId = null, string iconCssClass = null, int? categoryId = null, int? attributeId = null)
        {
            global::Rock.Model.Attribute attribute;
            AttributeController controller = new AttributeController(Service);
            if ( attributeId == null )
            {
                attribute = new global::Rock.Model.Attribute();
            }
            else
            {
                attribute = controller.GetById( (int)attributeId );
            }

            attribute.IsSystem = isSystem;
            attribute.FieldTypeId = fieldTypeId;
            attribute.EntityTypeId = entityTypeId;
            attribute.EntityTypeQualifierColumn = entityTypeQualifierColumn;
            attribute.EntityTypeQualifierValue = entityTypeQualifierValue;
            attribute.Key = key;
            attribute.Name = name;
            attribute.Description = description;
            attribute.Order = order;
            attribute.IsGridColumn = isGridColumn;
            attribute.DefaultValue = defaultValue;
            attribute.IsMultiValue = isMultiValue;
            attribute.IsRequired = isRequired;

            if ( guid != default( Guid ) )
            {
                attribute.Guid = guid;
            }

            attribute.ForeignId = foreignId;
            attribute.IconCssClass = iconCssClass;

            if ( attribute.Id > 0 )
            {
                attribute.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( attribute );
            }
            else
            {
                attribute.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( attribute );
            }

            attribute = controller.GetByGuid( attribute.Guid );

            //if ( categoryId != null )
            //{
            //    SaveAttributeCategory( attribute.Id, categoryId );
            //}

            return attribute.Id;
        }

        public int SaveAttributeValue( int attributeId, string value, int? entityId = null, bool isSystem = false, string foreignId = null, int? attributeValueId = null )
        {
            AttributeValuesController controller = new AttributeValuesController( Service );

            AttributeValue attributeValue;

            if ( attributeValueId != null )
            {
                attributeValue = controller.GetById( (int)attributeValueId );
            }
            else
            {
                attributeValue = new AttributeValue();
            }

            attributeValue.IsSystem = isSystem;
            attributeValue.AttributeId = attributeId;
            attributeValue.EntityId = entityId;
            attributeValue.Value = value;
            attributeValue.ForeignId = foreignId;

            if ( attributeValue.Id > 0 )
            {
                attributeValue.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( attributeValue );
            }
            else
            {
                attributeValue.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( attributeValue );
            }

            attributeValue = controller.GetByGuid( attributeValue.Guid );


            return attributeValue.Id;
            

        }

        private void SaveAttributeCategory( int p, int? categoryId )
        {
            throw new NotImplementedException();
        }



        

        #endregion

    }
}
