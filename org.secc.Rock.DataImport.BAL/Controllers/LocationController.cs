using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class LocationController : BaseController<Location>
    {
        string BaseAPIPath = "/api/Locations/";

        private LocationController() : base() { }

        public LocationController( RockService service ) : base( service ) { }

        public override void Add( Location entity )
        {
            Service.PostData<Location>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override Location GetById( int id )
        {
           return Service.GetData<Location>( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override Location GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Location>( BaseAPIPath, guid );
        }

        public override List<Location> GetAll()
        {
            return Service.GetData<List<Location>>( BaseAPIPath );
        }

        public override List<Location> GetByFilter( string expression )
        {
            return Service.GetData<List<Location>>( BaseAPIPath, expression );
        }

        public override Location GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( Location entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PostData<Location>( apiPath, entity );
        }
    }
}
