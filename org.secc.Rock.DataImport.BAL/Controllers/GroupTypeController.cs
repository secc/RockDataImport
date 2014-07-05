using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Rock.Model;
namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupTypeController : BaseController<GroupType>
    {
        private string BaseAPIPath = "/api/GroupTypes/";

        private GroupTypeController() : base() { }

        public GroupTypeController( RockService service ) : base( service ) { }

        public override void Add( GroupType entity )
        {
            Service.PostData<GroupType>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData(string.Format(BaseAPIPath + "{0}", id));
        }

        public override GroupType GetById( int id )
        {
            return Service.GetData<GroupType>( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override GroupType GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<GroupType>( BaseAPIPath, guid );
        }

        public override List<GroupType> GetAll()
        {
            return Service.GetData<List<GroupType>>( BaseAPIPath );
        }

        public override List<GroupType> GetByFilter( string expression )
        {
            return Service.GetData<List<GroupType>>( BaseAPIPath, expression );
        }

        public override GroupType GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( GroupType entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<GroupType>( apiPath, entity );
        }
    }
}
