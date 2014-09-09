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
    public class DefinedTypeMap : MapBase
    {
        RockService Service { get; set; }
        private DefinedTypeMap() { }

        public DefinedTypeMap( RockService service )
        {
            Service = service;
        }

        public DefinedTypeSummary GetDefinedTypeSummary( int id )
        {
            DefinedTypeController controller = new DefinedTypeController( Service );
            DefinedType definedType = controller.GetById( id );

            return LoadDefinedTypeSummary( definedType );
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

        public int? Save(string category, string name, string description = null, bool isSystem = false, int? fieldTypeId = 1, int order = 0, string foreignId = null, string helpText = null, int definedTypeId = 0 )
        {
            DefinedTypeController controller = new DefinedTypeController( Service );
            DefinedType definedType = null;

            if(definedTypeId > 0)
            {
                definedType = controller.GetById( definedTypeId );

                if(definedType == null || definedType == default(DefinedType))
                {
                    return null;
                }
            }
            else
            {
                definedType = new DefinedType(); 
            }

            definedType.IsSystem = isSystem;
            definedType.FieldTypeId = fieldTypeId;
            definedType.Order = order;
            definedType.Category = category;
            definedType.Name = name;
            definedType.Description = description;
            definedType.ForeignId = foreignId;
            definedType.HelpText = helpText;

            if ( definedTypeId > 0 )
            {
                definedType.ModifiedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Update( definedType );
            }
            else
            {
                definedType.CreatedByPersonAliasId = Service.LoggedInPerson.PrimaryAliasId;
                controller.Add( definedType );
            }


            definedType = controller.GetByGuid( definedType.Guid );

            return definedType.Id;
        }
    }
}
