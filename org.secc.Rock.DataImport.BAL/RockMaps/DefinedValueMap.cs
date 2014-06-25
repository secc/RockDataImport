using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
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

        internal DefinedValue GetDefinedValueByGuid( Guid guid )
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            DefinedValue dv = cache[string.Format( "DefinedValue_{0}", guid )] as DefinedValue;

            if ( dv == null )
            {
                DefinedValueController controller = new DefinedValueController( Service );
                dv = controller.GetByGuid( guid );

                cache.Set( string.Format( "DefinedValue_{0}", guid ), dv, policy );
            }

            return dv;
        }
    }
}
