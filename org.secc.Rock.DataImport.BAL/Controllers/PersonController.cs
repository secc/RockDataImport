using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PersonController
    {
        public static Person GetByUserName( RockService service, string userName )
        {
            string apiMethod = string.Format( "/api/People/GetByUserName/{0}", userName );

            return service.GetData<Person>( apiMethod );
        }

    }
}
