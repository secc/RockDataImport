using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using RestSharp;
using Rock.Model;
using Rock.Security;

namespace org.secc.Rock.DataImport.BAL
{
    public class RockService
    {
        RestClient Client { get; set; }
        Person LoggedInPerson { get; set; }

        public RockService() { }

        public RockService(string baseUrl)
        {
            Client = new RestClient(baseUrl);
            Client.CookieContainer = new System.Net.CookieContainer();
        }

        public void Login(string userName, string password)
        {
            RestRequest request = new RestRequest( "api/Auth/Login", Method.POST );
            request.RequestFormat = DataFormat.Json;
            request.AddBody( new LoginParameters() { Username = userName, Password = password } );
            var response = Client.Execute( request );

            if ( response.ErrorException != null )
            {
                throw response.ErrorException;
            }

            LoggedInPerson = Controllers.PersonController.GetByUserName( this, userName );

            if ( LoggedInPerson != null )
            {
                LoggedInPerson.Aliases = Controllers.PersonAliasController.GetByPersonId( this, LoggedInPerson.Id );
            }
        }


        public T  GetData<T>( string apiPath )
        {
            return GetData<T>( apiPath, null );
        }

        public T GetData<T>( string apiPath, string filterExpression )
        {
            var request = new RestRequest( apiPath, Method.GET );
            request.RequestFormat = DataFormat.Json;
           
            if(!String.IsNullOrWhiteSpace(filterExpression))
            {
                request.AddParameter( "$filter", filterExpression );
            }

            var response = Client.Execute( request );

            if ( response.StatusCode != System.Net.HttpStatusCode.OK )
            {
                throw new RockServiceException( response.StatusCode, "Unexpected Status Code returned.", response.ErrorException );
            }



            return JsonConvert.DeserializeObject<T>( response.Content );
        }


    }

    [Serializable]
    public class RockServiceException : Exception
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public RockServiceException() { }
        public RockServiceException( string message ) : base( message ) { }
        public RockServiceException( string message, Exception inner ) : base( message, inner ) { }
        public RockServiceException( System.Net.HttpStatusCode statusCode, string message, Exception inner )
            : base( message, inner )
        {
            StatusCode = statusCode;
        }
        protected RockServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }
}
