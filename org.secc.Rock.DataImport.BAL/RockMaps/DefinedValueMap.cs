using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class DefinedValueMap
    {
        public RockService Service { get; set; }

        private DefinedValueMap() { }

        public DefinedValueMap( RockService service )
        {
            Service = service;
        }

        public int? Save( int definedTypeId, bool isSystem, string name, int order, string description = null, string foreignId = null, int? id = null )
        {
            DefinedValue dv;
            DefinedValueController Controller = new DefinedValueController( Service );

            if ( id == null )
            {
                dv = Controller.GetById( (int)id );

                if ( dv == null )
                {
                    return null;
                }
            }
            else
            {
                dv = new DefinedValue();
            }

            throw new NotImplementedException();
        }
    }
}
