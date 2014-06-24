using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PhoneNumbersController : BaseController<PhoneNumber>
    {
        string BaseAPIPath = "api/PhoneNumbers/";

        private PhoneNumbersController() : base() { }

        public PhoneNumbersController( RockService service ) : base( service ) { }

        public override void Add( PhoneNumber entity )
        {
            Service.PostData<PhoneNumber>( BaseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseAPIPath + "{0}", id ) );
        }

        public override PhoneNumber GetById( int id )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", id );
            return Service.GetData<PhoneNumber>( apiPath );
        }

        public override PhoneNumber GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<PhoneNumber>( BaseAPIPath, guid );
        }

        public override List<PhoneNumber> GetAll()
        {
            return Service.GetData<List<PhoneNumber>>( BaseAPIPath );
        }

        public override List<PhoneNumber> GetByFilter( string expression )
        {
            return Service.GetData<List<PhoneNumber>>( BaseAPIPath, expression );
        }

        public override PhoneNumber GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( PhoneNumber entity )
        {
            string apiPath = string.Format( BaseAPIPath + "{0}", entity.Id );
            Service.PutData<PhoneNumber>( apiPath, entity );
        }

        public List<PhoneNumber> GetByPersonId( int personId )
        {
            string expression = string.Format( "PersonId eq {0}", personId );
            return GetByFilter( expression );
        }
    }
}
