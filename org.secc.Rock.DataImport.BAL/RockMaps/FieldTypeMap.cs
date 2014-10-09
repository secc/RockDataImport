using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class FieldTypeMap : MapBase
    {

        RockService Service;
        string cacheKey = "RockFieldTypes";

        private List<FieldType> FieldTypes
        {
            get
            {
                return GetCachedFieldTypes();
            }
            set
            {
                UpdateFieldTypeCache( value );
            }
        }

        #region Constructors
        private FieldTypeMap() { }

        public FieldTypeMap( RockService service )
        {
            Service = service;
        }

        #endregion

        #region Public Methods

        public int GetValueListFieldTypeId()
        {
            FieldType ft = GetFieldTypeByClassName( typeof( global::Rock.Field.Types.ValueListFieldType ).ToString() );

            return ft.Id;
        }

        #endregion


        #region Private Methods

        private List<FieldType> GetCachedFieldTypes()
        {
            ObjectCache cache = MemoryCache.Default;

            return (List<FieldType>)cache[cacheKey];
        }

        private FieldType GetFieldTypeByClassName( string className )
        {
            var fieldType = FieldTypes.Where( ft => ft.Class == className ).FirstOrDefault();

            if ( fieldType == null )
            {
                FieldTypeController controller = new FieldTypeController( Service );
                fieldType = controller.GetByClassName( className );

                if ( fieldType == null )
                {
                    FieldTypes.Add( fieldType );
                }
            }

            return fieldType;
        }

        private void UpdateFieldTypeCache( List<FieldType> ft )
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            cache.Set( cacheKey, ft, policy );
        }

        #endregion

    }
}
