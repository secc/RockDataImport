using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class CategoryMap : MapBase
    {
        RockService Service;


        #region Controllers
        private CategoryMap() { }

        public CategoryMap( RockService service )
        {
            Service = service;
        }
        #endregion

        #region Public Methods
        public Dictionary<string, object> GetByGuid( Guid guid )
        {
            CategoryController categoryController = new CategoryController( Service );

            return ToDictionary( categoryController.GetByGuid( guid ) );
        }

        public int SavePersonAttributeCategory( string name, bool isSystem, int? parentCategoryId = null, string description = null, string iconCssClass = null, int? order = null,  string foreignId = null, Guid guid = default(Guid), int? categoryId = null )
        {
            EntityTypeController etController = new EntityTypeController( Service );
            int attributeEntityTypeId = etController.GetByFriendlyName( typeof( global::Rock.Model.Attribute ).ToString() ).Id;
            int personEntityTypeId = etController.GetByFriendlyName( typeof( Person ).ToString() ).Id;

            return SaveCategory( 
                    isSystem: isSystem,
                    parentCategoryId: parentCategoryId,
                    entityTypeId: attributeEntityTypeId,
                    entityTypeQualifierColumn: "EntityTypeId",
                    entityTypeQualifierValue: personEntityTypeId.ToString(),
                    name: name,
                    iconCssClass: iconCssClass,
                    order: order,
                    description: description,
                    foreignId: foreignId,
                    categoryId: categoryId
                    );
            

        }

        public int SaveCategory( int entityTypeId, string name, int? order = null, bool isSystem = false, int? parentCategoryId = null, string description = null,
            string entityTypeQualifierColumn = null, string entityTypeQualifierValue = null, string iconCssClass = null, string highlightColor = null, string foreignId = null, Guid guid = default(Guid), int? categoryId = null )
        {
            CategoryController controller = new CategoryController( Service );
            Category category;


            if ( categoryId != null )
            {
                category = controller.GetById( (int)categoryId );
            }
            else
            {
                category = new Category();
            }

            category.IsSystem = isSystem;
            category.ParentCategoryId = parentCategoryId;
            category.EntityTypeId = entityTypeId;
            category.EntityTypeQualifierColumn = entityTypeQualifierColumn;
            category.EntityTypeQualifierValue = entityTypeQualifierValue;
            category.Name = name;
            category.IconCssClass = iconCssClass;

            if ( order == null )
            {
                order = GetNextOrderValueByEntityType( entityTypeId );
            }
            category.Order = (int)order;


            if ( guid != default( Guid ) )
            {
                category.Guid = guid;
            }

            category.Description = description;
            category.ForeignId = foreignId;
            category.HighlightColor = highlightColor;

            if ( category.Id == 0 )
            {
                category.CreatedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Add( category );
            }
            else
            {
                category.ModifiedByPersonAliasId = Service.GetCurrentPersonAliasId();
                controller.Update( category );
            }

            return controller.GetByGuid( category.Guid ).Id;

        }

        private int GetNextOrderValueByEntityType(int entityTypeId)
        {
            CategoryController controller = new CategoryController(Service);
            int order = 0;
            string filter = string.Format( "EntityTypeId eq {0} &$orderby=order desc&top=1", entityTypeId );

            var entityType = controller.GetByFilter( filter ).FirstOrDefault();

            if ( entityType != null )
            {
                order = entityType.Order;
            }

            return order++;
            
        }
        #endregion

    }
}
