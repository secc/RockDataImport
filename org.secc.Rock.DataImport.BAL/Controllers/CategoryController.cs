using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class CategoryController : BaseController<Category>
    {
        private string BaseApiPath = "/api/Categories/";

        private CategoryController() : base() { }

        public CategoryController( RockService service ) : base( service ) { }

        public override void Add( Category entity )
        {
            Service.PostData<Category>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            Service.DeleteData( apiPath );
        }

        public override Category GetById( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            return Service.GetData<Category>( apiPath );
        }

        public override Category GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Category>( BaseApiPath, guid );
        }

        public override List<Category> GetAll()
        {
            return Service.GetData<List<Category>>( BaseApiPath );
        }

        public override List<Category> GetByFilter( string expression )
        {
            return Service.GetData<List<Category>>( BaseApiPath, expression );
        }

        public override Category GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( Category entity )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, entity.Id );
            Service.PutData<Category>( apiPath, entity );
        }

        public List<Category> GetByEntityType( Type entityType )
        {
            return GetByEntityTypeQualifier( entityType );
        }

        public List<Category> GetByEntityTypeQualifier( Type entityType, string qualifierColumn = null, string qualifierValue = null )
        {
            EntityTypeController etController = new EntityTypeController( Service );
            EntityType et = etController.GetByFriendlyName( entityType.ToString() );

            if ( et == null )
            {
                return new List<Category>();
            }
            else
            {
                StringBuilder filterBuilder = new StringBuilder();
                filterBuilder.AppendFormat( "EntityTypeId eq {0}", et.Id );

                if ( !String.IsNullOrWhiteSpace( qualifierColumn ) )
                {
                    filterBuilder.AppendFormat( " and EntityTypeQualifierColumn eq '{0}'", qualifierColumn );
                }

                if ( !String.IsNullOrWhiteSpace( qualifierValue ) )
                {
                    filterBuilder.AppendFormat( " and EntityTypeQualifierValue eq '{0}'", qualifierValue );
                }

                return GetByFilter( filterBuilder.ToString() );

            }
        }
    }
}
