using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class DefinedValueController : BaseController<DefinedValue>
    {
        string baseApiPath = "/api/DefinedValues/";

        private DefinedValueController() : base() { }

        public DefinedValueController( RockService service ) : base( service ) { }


        public override void Add( DefinedValue entity )
        {
            Service.PostData<DefinedValue>( baseApiPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( baseApiPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override DefinedValue GetById( int id )
        {
            string apiPath = string.Format( baseApiPath + "0", id );
           return Service.GetData<DefinedValue>( apiPath );
        }

        public override DefinedValue GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<DefinedValue>( baseApiPath, guid );
        }

        public override List<DefinedValue> GetAll()
        {
            return Service.GetData<List<DefinedValue>>( baseApiPath );
        }

        public override List<DefinedValue> GetByFilter( string expression )
        {
            return Service.GetData<List<DefinedValue>>( baseApiPath, expression );
        }

        public override DefinedValue GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq {0}", foreignId );
            return (Service.GetData<List<DefinedValue>>( baseApiPath, expression  )).FirstOrDefault();
        }

        public override void Update( DefinedValue entity )
        {
            string apiPath = string.Format( baseApiPath + "{0}", entity.Id );
            Service.PutData<DefinedValue>( apiPath, entity );
        }

        public  List<DefinedValue> GetByDefinedTypeId( int definedTypeId )
        {
            string expression = string.Format( string.Format( "DefinedTypeId eq {0}", definedTypeId ) );
            return GetByFilter( expression );
        }

        public List<DefinedValue> GetByDefinedTypeGuid( Guid definedTypeGuid )
        {
            DefinedTypeController definedTypeController = new DefinedTypeController( Service );
            var definedType = definedTypeController.GetByGuid( definedTypeGuid );

            if ( definedType != null )
            {
                string expression = string.Format( "DefinedTypeId eq {0}", definedType.Id );
                return GetByFilter( expression );
            }
            else
            {
                return null;
            }

        }


    }
}
