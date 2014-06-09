using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class CampusController : BaseController<Campus>
    {
        private CampusController() : base() { }

        public CampusController( RockService service ) : base( service ) { }

        public string baseAPIPath = "/api/Campus/";

        public override void Add( Campus entity )
        {
            Service.PostData<Campus>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override Campus GetById( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            return Service.GetData<Campus>( apiPath );
        }

        public override Campus GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Campus>( baseAPIPath, guid );
        }

        public override List<Campus> GetAll()
        {
            return Service.GetData<List<Campus>>( baseAPIPath );
        }

        public override List<Campus> GetByFilter(string expression)
        {
            return Service.GetData<List<Campus>>( baseAPIPath, expression );
        }

        public override Campus GetByForeignKey( string foreignKeyValue )
        {
            string filter = string.Format( "foreignkey eq '{0}'", foreignKeyValue );
            return ( Service.GetData<List<Campus>>( baseAPIPath, filter ) ).FirstOrDefault();
        }

        public override void Update( Campus entity )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", entity.Id );
            Service.PutData<Campus>( apiPath, entity );
        }
    }

}
