using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Net;

namespace org.secc.Rock.DataImport.BAL.Service
{
    public abstract class RestService
    {
        public RockRestClient RestClient { get; set; }

        public RestService() { }

        public RestService(RockRestClient client)
        {
            RestClient = client;      
        }
    }
}
