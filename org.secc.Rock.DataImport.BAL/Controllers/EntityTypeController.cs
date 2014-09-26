using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class EntityTypeController : BaseController<EntityType>
    {
        string BaseApiPath = "/api/EntityTypes/";

        public EntityTypeController() : base() {}

        public EntityTypeController( RockService service ) : base( service ) { }

        public override EntityType GetById( int id )
        {
            string apiPath = string.Format( BaseApiPath + "{0}", id.ToString() );
            return Service.GetData<EntityType>( apiPath );
        }


        public override void Add( EntityType entity )
        {
            Service.PostData<EntityType>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseApiPath + "{0}", id ) );
        }

        public override EntityType GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<EntityType>( BaseApiPath, guid );
        }

        public override List<EntityType> GetAll()
        {
            return Service.GetData<List<EntityType>>( BaseApiPath );
        }

        public override List<EntityType> GetByFilter( string expression )
        {
            return Service.GetData<List<EntityType>>( BaseApiPath, expression );
        }

        public override EntityType GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return ( GetByFilter( expression ) ).FirstOrDefault();
        }

        public override void Update( EntityType entity )
        {
            string apiPath = string.Format( BaseApiPath + "{0}", entity.Id );
            Service.PutData<EntityType>( apiPath, entity );
        }
    }
}
