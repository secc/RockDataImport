using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Net;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Service
{
    public class PersonRestService : RestService
    {
        private PersonRestService() { }

        public PersonRestService( RockRestClient client ) : base( client ) { }

        public Person GetByUserName( string userName )
        {
            string apiMethodCall = string.Format( "api/People/GetByUserName/{0}", userName );

            return RestClient.GetData <Person>( apiMethodCall );
        }

        public List<PersonAlias> GetPersonAliases( int personId )
        {
            string apiMethodCall = "api/PersonAlias/";
            string filter = string.Format( "PersonId eq {0}", personId );

            return RestClient.GetData<List<PersonAlias>>( apiMethodCall, filter );

        }

    }
}
