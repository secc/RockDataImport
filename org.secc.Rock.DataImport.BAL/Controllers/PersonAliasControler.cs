using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PersonAliasController
    {

        public static List<PersonAlias> GetByPersonId(RockService service, int personId)
        {
            string apiPath = string.Format( "api/PersonAlias/" );
            string filterExpression = string.Format( "PersonId eq {0}", personId );

            return service.GetData<List<PersonAlias>>( apiPath, filterExpression );
        }
    }
}
