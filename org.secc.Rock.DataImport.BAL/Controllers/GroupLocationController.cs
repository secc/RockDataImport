using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class GroupLocationController : BaseController<GroupLocation>
    {
        string BaseAPIPath = "/api/GroupLocations/";

        private GroupLocationController() : base() { }

        public GroupLocationController( RockService service ) : base( service ) { }

        public override void Add( GroupLocation entity )
        {
            Service.PostData<GroupLocation>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override GroupLocation GetById( int id )
        {
            return Service.GetData<GroupLocation>( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override GroupLocation GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<GroupLocation>( BaseAPIPath, guid );
        }

        public override List<GroupLocation> GetAll()
        {
            return Service.GetData<List<GroupLocation>>( BaseAPIPath );
        }

        public override List<GroupLocation> GetByFilter( string expression )
        {
            return Service.GetData<List<GroupLocation>>( BaseAPIPath, expression );
        }

        public override GroupLocation GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( GroupLocation entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<GroupLocation>( BaseAPIPath, entity );
        }

        public List<GroupLocation> GetByGroupId( int groupId )
        {
            string expression = string.Format( "GroupId eq {0}", groupId );
            return GetByFilter( expression );
        }
    }
}
