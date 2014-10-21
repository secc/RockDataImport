using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PersonController :BaseController<Person>
    {
        string baseAPIPath = "/api/People/";

        private PersonController() { }

        public PersonController( RockService service )
        {
            Service = service;
        }

        public override void Add( Person entity )
        {
            Service.PostData<Person>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( baseAPIPath + "{0}", id ) );
        }

        public override Person GetById( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            return Service.GetData<Person>( apiPath );
        }

        public override Person GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Person>( baseAPIPath, guid );
        }

        public Person GetByGuid(Guid guid, bool includeDeceased)
        {
            Dictionary<string, string> queryString = null;

            if ( includeDeceased )
            {
                queryString = new Dictionary<string, string>();
                queryString.Add( "IncludeDeceased", Boolean.TrueString );
            }

            return Service.GetDataByGuid<Person>( baseAPIPath, guid, queryString );
        }

        public override List<Person> GetAll()
        {
            return Service.GetData<List<Person>>( baseAPIPath );
        }

        public override List<Person> GetByFilter( string expression )
        {
            return Service.GetData<List<Person>>( baseAPIPath, expression );
        }

        public List<Person> GetByFilter( string expression, bool includeDeceased )
        {
            Dictionary<string, string> queryString = null;

            if ( includeDeceased )
            {
                queryString = new Dictionary<string, string>();
                queryString.Add( "IncludeDeceased", Boolean.TrueString );
            }

            return Service.GetData<List<Person>>( baseAPIPath, expression, queryString );
        }

        public override Person GetByForeignId( string foreignId )
        {
            return GetByForeignId( foreignId, false );
        }

        public Person GetByForeignId( string foreignId, bool includeDeceased )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return ( GetByFilter( expression, includeDeceased ) ).FirstOrDefault();
        }

        public override void Update( Person entity )
        {
            string apiPath = string.Format( "api/People/{0}", entity.Id );
            Service.PutData<Person>( apiPath, entity );
        }

        public Person GetByUserName( string userName )
        {
            string apiMethod = string.Format( baseAPIPath + "GetByUserName/{0}", userName );

            return Service.GetData<Person>( apiMethod );
        }
    }
}
