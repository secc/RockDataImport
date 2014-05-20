using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;

namespace ConsoleTest
{
    class ConnectionTest
    {
        static void Main( string[] args )
        {
            try
            {
                string url = "http://internal.rockbeta.secc.org";
                //string url = "http://localhost:6229";
                string user = "admin";
                string pass = "admin1";

                RockConnection connection = new RockConnection();
                connection.Connect( url, user, pass );

                Console.WriteLine( String.Format( "{0} is logged in.", connection.LoggedInPerson.FullName ) );


            }
            catch ( System.Net.WebException ex )
            {
                System.Net.HttpWebResponse response = ex.Response as System.Net.HttpWebResponse;

                if ( response != null && response.StatusCode.Equals( System.Net.HttpStatusCode.Unauthorized ) )
                {
                    Console.WriteLine( "Invalid Login" );
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine( ex.Message );

                    if ( ex.InnerException != null )
                    {
                        sb.AppendLine( ex.InnerException.Message );
                    }

                    Console.Write( sb.ToString() );
                }
                
            }
            Console.ReadKey();


        }
    }
}
