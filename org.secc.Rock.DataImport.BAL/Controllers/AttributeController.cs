using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RockModel = Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class AttributeController : BaseController<RockModel.Attribute>
    {
        string BaseAPIPath = "/api/Attributes";

        private AttributeController() : base() { }

        public AttributeController( RockService service ) : base( service ) { }



        public override void Add( RockModel.Attribute entity )
        {
            Service.PostData<RockModel.Attribute>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override RockModel.Attribute GetById( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            return Service.GetData<RockModel.Attribute>( apiPath );
        }

        public override RockModel.Attribute GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<RockModel.Attribute>( BaseAPIPath, guid );
        }

        public override List<RockModel.Attribute> GetAll()
        {
            return Service.GetData<List<RockModel.Attribute>>( BaseAPIPath );
        }

        public override List<RockModel.Attribute> GetByFilter( string expression )
        {
            return Service.GetData<List<RockModel.Attribute>>( BaseAPIPath, expression );
        }

        public override RockModel.Attribute GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( RockModel.Attribute entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<RockModel.Attribute>( apiPath, entity );
        }

        public void FlushAll()
        {
            string apiPath = BaseAPIPath + "flush";
            Service.PutData<RockModel.Attribute>( apiPath, null );
        }

        public void FlushAttribute( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "flush/{0}", id );
            Service.PutData<RockModel.Attribute>( apiPath, null );
        }
    }
}
