using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupTypeRoleController : BaseController<GroupTypeRole> 
    {
        string BaseAPIPath = "/api/GroupTypeRoles/";

        private GroupTypeRoleController() : base() { }

        public GroupTypeRoleController( RockService service ) : base( service ) { }

        public override void Add( GroupTypeRole entity )
        {
            Service.PostData<GroupTypeRole>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override GroupTypeRole GetById( int id )
        {
            return Service.GetData<GroupTypeRole>( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override GroupTypeRole GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<GroupTypeRole>( BaseAPIPath, guid );
        }

        public override List<GroupTypeRole> GetAll()
        {
            return Service.GetData<List<GroupTypeRole>>( BaseAPIPath );
        }

        public override List<GroupTypeRole> GetByFilter( string expression )
        {
            return Service.GetData<List<GroupTypeRole>>( BaseAPIPath, expression );
        }

        public override GroupTypeRole GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq {0}", foreignId );
            return GetByFilter( foreignId ).FirstOrDefault();
        }

        public override void Update( GroupTypeRole entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<GroupTypeRole>( apiPath, entity );
        }
    }
}
