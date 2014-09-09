using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Data;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class MapBase
    {
        internal Dictionary<string, object> ToDictionary( IEntity entityObject )
        {
            if ( entityObject == null )
            {
                return null;
            }

            var dictionary = new Dictionary<string, object>();
            string[] addlProperties = new string[] { "Id", "Guid", "Order", "ForeignId", "CreatedByPersonAliasId", "ModifiedByPersonAliasId", "CreatedDateTime", "ModifiedDateTime" };
            foreach ( var prop in entityObject.GetType().GetProperties() )
            {
                if ( !prop.GetGetMethod().IsVirtual || addlProperties.Contains( prop.Name ) )
                {
                    dictionary.Add( prop.Name, prop.GetValue( entityObject, null ) );
                }
            }
            return dictionary;
        }
    }
}
