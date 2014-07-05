using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class AttributeQualifiersController : BaseController<AttributeQualifier>
    {
        string BaseAPIPath = "/api/AttributeQualifiers/";

        private AttributeQualifiersController() : base() { }

        public AttributeQualifiersController( RockService service ) : base( service ) { }

        public override void Add( AttributeQualifier entity )
        {
            Service.PostData<AttributeQualifier>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData(string.Format(BaseAPIPath + "{0}", id));
        }

        public override AttributeQualifier GetById( int id )
        {
            return Service.GetData<AttributeQualifier>( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override AttributeQualifier GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<AttributeQualifier>( BaseAPIPath, guid );
        }

        public override List<AttributeQualifier> GetAll()
        {
            return Service.GetData<List<AttributeQualifier>>( BaseAPIPath );
        }

        public override List<AttributeQualifier> GetByFilter( string expression )
        {
            return Service.GetData<List<AttributeQualifier>>( BaseAPIPath, expression );
        }

        public override AttributeQualifier GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( AttributeQualifier entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<AttributeQualifier>( apiPath, entity );
        }

    }
}
