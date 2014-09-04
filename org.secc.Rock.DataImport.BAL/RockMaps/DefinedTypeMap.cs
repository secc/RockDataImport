using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using org.secc.Rock.DataImport.BAL.Helper;
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
            return LoadDefinedTypeSummary( definedType );

        }

        public DefinedTypeSummary GetDefinedTypeSummaryByForeignId( string foreignId )
        {
            DefinedTypeController controller = new DefinedTypeController(Service);
            DefinedType definedType = controller.GetByForeignId( foreignId );

            if ( definedType == null )
            {
                return null;
            }

            return LoadDefinedTypeSummary( definedType );
        }

        public DefinedTypeSummary LoadDefinedTypeSummary( DefinedType dt )
        {
            DefinedValueController definedValueController = new DefinedValueController( Service );

            dt.DefinedValues = definedValueController.GetByDefinedTypeId( dt.Id );

            return new DefinedTypeSummary( dt );
        }
    }
}
