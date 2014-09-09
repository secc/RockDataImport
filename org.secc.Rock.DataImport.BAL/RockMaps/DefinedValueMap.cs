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
    public class DefinedValueMap : MapBase
    {
        public RockService Service { get; set; }

        private DefinedValueMap() { }

        public DefinedValueMap( RockService service )
        {
            Service = service;
        }

        public int? Save( int definedTypeId, string value, string description = null, bool isSystem = false, int order = 0, string foreignId = null, int definedValueId = 0 )
        {
            DefinedValueController controller = new DefinedValueController( Service );
            DefinedValue dv = null;

            if ( definedValueId > 0 )
            {
                dv = controller.GetById( definedValueId );

                if ( dv == null )
                {
                    return null;
                }
            }
            else
            {
                dv = new DefinedValue();
            }

            dv.IsSystem = isSystem;
            dv.DefinedTypeId = definedTypeId;
            dv.Order = order;
            dv.Value = value;
            dv.Description = description;
            dv.ForeignId = foreignId;

            if ( definedValueId == 0 )
            {
                dv.CreatedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Add( dv );
            }
            else
            {
                dv.ModifiedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Update( dv );
            }

            return controller.GetByGuid( dv.Guid ).Id;


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
