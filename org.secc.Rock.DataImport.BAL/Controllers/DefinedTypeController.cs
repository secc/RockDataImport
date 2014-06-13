using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class DefinedTypeController : BaseController<DefinedType>
    {
        string baseAPIPath = "/api/DefinedTypes/";

        private DefinedTypeController() : base() { }

        public DefinedTypeController( RockService service ) : base( service ) { }

        public override void Add( DefinedType entity )
        {
            Service.PostData<DefinedType>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override DefinedType GetById( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            return Service.GetData<DefinedType>( apiPath );
        }

        public override DefinedType GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<DefinedType>( baseAPIPath, guid );
        }

        public override List<DefinedType> GetAll()
        {
            return Service.GetData<List<DefinedType>>( baseAPIPath );
        }

        public override List<DefinedType> GetByFilter( string expression )
        {
            return Service.GetData<List<DefinedType>>( baseAPIPath, expression );
        }

        public override DefinedType GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq {0}", foreignId );
            return ( Service.GetData<List<DefinedType>>( baseAPIPath ) ).FirstOrDefault();
        }

        public override void Update( DefinedType entity )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", entity.Id );
            Service.PutData<DefinedType>( apiPath, entity );
        }
    }
}
