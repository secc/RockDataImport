using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Service;

using Rock.Net;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL
{
    public class RockConnection
    {
        RockRestClient mClient = null;
        
        public RockRestClient Client 
        {
            get
            {
                return mClient;
            }
            set
            {
                mClient = value;
            }
        }

        public Person LoggedInPerson{get;set;}

        public void Connect(string url, string userName, string password)
        {
            if(String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url", "URL is required to connect to Rock");
            }

            if(String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName", "User Name is required.");
            }

            if(String.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password", "Password is required.");
            }

            Client = new RockRestClient(url);
            Client.Login(userName, password);

            var service = new PersonRestService(Client);

            LoggedInPerson = service.GetByUserName(userName);

            if(LoggedInPerson != null)
            {
                LoggedInPerson.Aliases = service.GetPersonAliases( LoggedInPerson.Id );
            }
        }
    }
        
}
