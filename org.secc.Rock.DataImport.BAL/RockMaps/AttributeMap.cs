using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class AttributeMap : MapBase
    {
        RockService Service;

        #region Constructor
        private AttributeMap() : base() { }

        public AttributeMap( RockService service )
        {
            Service = service;
        }
        #endregion

        #region Public
        public Dictionary<string, object> GetByGuid( Guid guid )
        {
            AttributeController controller = new AttributeController( Service );

            return ToDictionary( controller.GetByGuid( guid ) );
        }

        public int SavePersonAttribute( int fieldTypeId, string key, string name, string description = null, int? order = null, bool isGridColumn = false,
                string defaultValue = null, bool isMultiValue = false, string foreignId = null, string iconCssClass = null,
                bool isSystem = false, Guid guid = default(Guid), int? attributeId = null )
        {
            EntityTypeController controller = new EntityTypeController( Service );
            int entityTypeId = controller.GetByFriendlyName( typeof( Person ).ToString() ).Id;

            throw new NotImplementedException();

        }

        public int SaveAttribute()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
