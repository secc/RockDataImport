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

        /// <summary>
        /// Gets or sets the person who is logged in to the Rock API.
        /// </summary>
        /// <value>
        /// The person who is logged in to the Rock API.
        /// </value>
        public Person LoggedInPerson { get; set; }

        /// <summary>
        /// Gets or sets Rest based client that is used to connect to the Rock API.
        /// </summary>
        /// <value>
        /// The Rest client that is use to connect to the Rock API.
        /// </value>
        private RestClient Client { get; set; }


        /// <summary>
        /// Gets or sets the username of the person who logged in to the Rock API.
        /// </summary>
        /// <value>
        /// The username of the person who logged in to the Rock API.
        /// </value>
        private string UserName { get; set; }


        /// <summary>
        /// Gets or sets the password that was used to log in to the Rock API
        /// </summary>
        /// <value>
        /// The password that was used to log in to the Rock API.
        /// </value>
        private string Password { get; set; }


        /// <summary>
        /// The retry attempt counter.
        /// </summary>
        private int attemptCount = 0;

        /// <summary>
        /// Prevents a default instance of the <see cref="RockService"/> class from being created.
        /// </summary>
        private RockService() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RockService"/> class and initializes the client.
        /// </summary>
        /// <param name="baseUrl">The base URL of the Rock endpoint.</param>
        public RockService(string baseUrl)
        {
            InitializeRestClient( baseUrl );
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RockService"/> class and attempts to login to the Rock API.
        /// </summary>
        /// <param name="baseUrl">The base URL of the Rock API endpoint.</param>
        /// <param name="userName">The UserName to authenticate to the Rock API.</param>
        /// <param name="password">The password to authenticate.</param>
        public RockService(string baseUrl, string userName, string password)
        {
            UserName = userName;
            Password = password;

            InitializeRestClient( baseUrl );

            Login();
        }


        public void DeleteData( string apiPath)
        {
            var request = new RestRequest( apiPath, Method.DELETE );
            request.RequestFormat = DataFormat.Json;
            var response = Client.Execute( request );

            if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized && attemptCount == 0 )
            {
                attemptCount++;
                Login();
                DeleteData( apiPath );
            }
            else if ( response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                attemptCount = 0;
            }
            else
            {
                throw new RockServiceException( response.StatusCode, "Unexpected Status Code returned.", response.ErrorException );
            }
        }


        /// <summary>
        /// Performs a Get request against the Rock API
        /// </summary>
        /// <typeparam name="T">The Rock entity to return.</typeparam>
        /// <param name="apiPath">The API method path.</param>
        /// <returns>The requested object.</returns>
        public T  GetData<T>( string apiPath )
        {
            return GetData<T>( apiPath, null );
        }

        /// <summary>
        /// Performs a Get request against the Rock API
        /// </summary>
        /// <typeparam name="T">The entity object or collection of entity objects to return.</typeparam>
        /// <param name="apiPath">The path to the API method.</param>
        /// <param name="filterExpression">The OData filter.</param>
        /// <returns> The requested data.</returns>
        /// <exception cref="org.secc.Rock.DataImport.BAL.RockServiceException">Unexpected Status Code returned.</exception>
        public T GetData<T>( string apiPath, string filterExpression )
        {
            T Data = default( T );
            var request = new RestRequest( apiPath, Method.GET );
            request.RequestFormat = DataFormat.Json;
           
            if(!String.IsNullOrWhiteSpace(filterExpression))
            {
                request.AddParameter( "$filter", filterExpression );
            }

            var response = Client.Execute( request );

            if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                attemptCount = 0; //reset attempt counter
                Data = JsonConvert.DeserializeObject<T>( response.Content );
            }
            else if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized && attemptCount == 0 )
            {
                attemptCount++;
                Login();
                Data = GetData<T>( apiPath, filterExpression );

            }
            else
            {
                throw new RockServiceException( response.StatusCode, "Unexpected Status Code returned.", response.ErrorException );
            }

            return Data;
        }

        public T GetDataByGuid<T>(string apiPath, Guid guid)
        {
            string filter = string.Format( "Guid eq '{0}'", guid );
            return GetData<List<T>>( apiPath, filter ).FirstOrDefault();

        }

        public void  PostData<T>( string apiPath, T entity )
        {
            var request = new RestRequest( apiPath, Method.POST );
            request.RequestFormat = DataFormat.Json;
            request.AddBody(entity);
            var response = Client.Execute( request );

            if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                attemptCount++;
                Login();
                PostData<T>( apiPath, entity );
            }
            else if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                attemptCount = 0;
            }
            else
            {
                throw new RockServiceException( response.StatusCode, "Unexpected Status Code returned.", response.ErrorException );
            }

        }

        public void PutData<T>( string apiPath, T entity )
        {
            var request = new RestRequest( apiPath, Method.PUT );
            request.RequestFormat = DataFormat.Json;
            request.AddBody( entity );
            var response = Client.Execute( request );

            if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized && attemptCount == 0 )
            {
                attemptCount++;
                Login();
                PutData<T>( apiPath, entity );
            }
            else if ( response.StatusCode == System.Net.HttpStatusCode.OK )
            {
                attemptCount = 0;
            }
            else
            {
                throw new RockServiceException( response.StatusCode, "Unexpected Status Code returned.", response.ErrorException );
            }
        }

        /// <summary>
        /// Sets the user's credentials and logs them into the Rock API.
        /// </summary>
        /// <param name="userName">The username</param>
        /// <param name="passWord">The password.</param>
        public void Login(string userName, string passWord)
        {
            UserName = userName;
            Password = passWord;
            Login();
        }

        /// <summary>
        /// Initializes the rest client.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        private void InitializeRestClient(string baseUrl)
        {
            Client = new RestClient( baseUrl );
            Client.CookieContainer = new System.Net.CookieContainer();
        }

        /// <summary>
        /// Logs the user into the Rock API
        /// </summary>
        /// <exception cref="org.secc.Rock.DataImport.BAL.RockServiceException">
        /// Username must be provided to login to Rock API.
        /// or
        /// Password must be provided to login to Rock API.
        /// </exception>
        private void Login()
        {

            if ( String.IsNullOrEmpty( UserName ) )
            {
                throw new RockServiceException( "Username must be provided to login to Rock API." );
            }

            if ( String.IsNullOrEmpty( Password ) )
            {
                throw new RockServiceException( "Password must be provided to login to Rock API." );
            }

            RestRequest request = new RestRequest( "api/Auth/Login", Method.POST );
            request.RequestFormat = DataFormat.Json;
            request.AddBody( new LoginParameters() { Username = UserName, Password = Password } );
            var response = Client.Execute( request );

            if ( response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.NoContent )
            {
                throw new RockServiceException( response.StatusCode, string.Format( "Unexpected status code returned. {0} - {1}.", (int)response.StatusCode, response.StatusDescription ), response.ErrorException );
            }

            LoggedInPerson = Controllers.PersonController.GetByUserName( this, UserName );

            if ( LoggedInPerson != null )
            {
                Controllers.PersonAliasController AliasController = new Controllers.PersonAliasController(this);
                LoggedInPerson.Aliases =  AliasController.GetByPersonId(LoggedInPerson.Id );
            }
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
