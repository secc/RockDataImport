using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupMembersController :BaseController<GroupMember>
    {
        string BaseAPIPath = "/api/GroupMembers/";

        public GroupMembersController() : base() { }

        public GroupMembersController( RockService service ) : base( service ) { }

        public override void Add( GroupMember entity )
        {
            Service.PostData<GroupMember>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override GroupMember GetById( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            return Service.GetData<GroupMember>( apiPath );
        }

        public override GroupMember GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<GroupMember>( BaseAPIPath, guid );
        }

        public override List<GroupMember> GetAll()
        {
            return Service.GetData<List<GroupMember>>( BaseAPIPath );
        }

        public override List<GroupMember> GetByFilter( string expression )
        {
            return Service.GetData<List<GroupMember>>( BaseAPIPath, expression );
        }

        public override GroupMember GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );

            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( GroupMember entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<GroupMember>( apiPath, entity );
        }

        public List<GroupMember> GetByGroupId( int groupId )
        {
            string expression = string.Format( "GroupId eq {0}", groupId );
            return GetByFilter( expression );
        }
    }
}
