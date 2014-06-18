using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using org.secc.Rock.DataImport.BAL.Model;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class DefinedTypeMap
    {
        RockService Service { get; set; }
        private DefinedTypeMap() { }

        public DefinedTypeMap( RockService service )
        {
            Service = service;
        }


        public DefinedTypeSummary GetDefinedTypeSummary( Guid definedTypeGuid )
        {
            DefinedTypeController definedTypeController = new DefinedTypeController( Service );

            DefinedType definedType = definedTypeController.GetByGuid( definedTypeGuid );

            if ( definedType == null )
            {
                return null;
            }
            DefinedValueController definedValueController = new DefinedValueController(Service);

            definedType.DefinedValues = definedValueController.GetByDefinedTypeId( definedType.Id );

            return new DefinedTypeSummary( definedType );

        }
    }
}
