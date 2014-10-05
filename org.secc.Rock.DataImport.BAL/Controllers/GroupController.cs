using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupController : BaseController<Group>
    {
        string BaseApiPath = "api/Groups/";
        private GroupController() : base() { }
        public GroupController( RockService service ) : base( service ) { }


        public override void Add( Group entity )
        {
            Service.PostData<Group>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseApiPath + "{0}", id ) );
        }

        public override Group GetById( int id )
        {
            return Service.GetData<Group>( string.Format( BaseApiPath + "{0}", id ) );
        }

        public override Group GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Group>( BaseApiPath, guid );
        }

        public override List<Group> GetAll()
        {
            return Service.GetData<List<Group>>( BaseApiPath );
        }

        public override List<Group> GetByFilter( string expression )
        {
            return Service.GetData<List<Group>>( BaseApiPath, expression );
        }

        public override Group GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return ( Service.GetData<List<Group>>( BaseApiPath, expression ) ).FirstOrDefault();
        }

        public Group GetByForeignIdGroupType( string foreignId, int groupTypeId )
        {
            string expression = string.Format( "ForeignId eq '{0}' and GroupTypeId eq {1}", foreignId, groupTypeId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( Group entity )
        {
            string apiPath = string.Format( BaseApiPath + "{0}", entity.Id );
            Service.PutData<Group>( apiPath, entity );
        }
    }
}
