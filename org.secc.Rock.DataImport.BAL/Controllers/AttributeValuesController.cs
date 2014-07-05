using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class AttributeValuesController : BaseController<AttributeValue>
    {
        string baseAPIPath = "/api/AttributeValues/";
        public override void Add( AttributeValue entity )
        {
            Service.PostData<AttributeValue>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( baseAPIPath + "{0}", id ) );
        }

        public override AttributeValue GetById( int id )
        {
            return Service.GetData<AttributeValue>( string.Format( baseAPIPath + "{0}", id ) );
        }

        public override AttributeValue GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<AttributeValue>( baseAPIPath, guid );
        }

        public override List<AttributeValue> GetAll()
        {
            return Service.GetData<List<AttributeValue>>( baseAPIPath );
        }

        public override List<AttributeValue> GetByFilter( string expression )
        {
            return Service.GetData<List<AttributeValue>>( baseAPIPath, expression );
        }

        public override AttributeValue GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( AttributeValue entity )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", entity.Id );
            Service.PostData<AttributeValue>( apiPath, entity );
        }
    }
}
