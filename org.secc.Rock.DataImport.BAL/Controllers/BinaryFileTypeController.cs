using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;
namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class BinaryFileTypeController : BaseController<BinaryFileType>
    {
        string BaseApiPath = "api/BinaryFileTypes/";

        private BinaryFileTypeController() : base() { }

        public BinaryFileTypeController( RockService service ) : base( service ) { }

        public override BinaryFileType GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<BinaryFileType>( BaseApiPath, guid );
        }

        public override void Add( BinaryFileType entity )
        {
            Service.PostData<BinaryFileType>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( BaseApiPath + "{0}", id ) );
        }

        public override BinaryFileType GetById( int id )
        {
            return Service.GetData<BinaryFileType>( string.Format( BaseApiPath + "{0}", id ) );
        }

        public override List<BinaryFileType> GetAll()
        {
            return Service.GetData<List<BinaryFileType>>( BaseApiPath );
        }

        public override List<BinaryFileType> GetByFilter( string expression )
        {
            return Service.GetData<List<BinaryFileType>>( BaseApiPath );
        }

        public override BinaryFileType GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return ( GetByFilter( expression ) ).FirstOrDefault();
        }

        public override void Update( BinaryFileType entity )
        {
            string apiPath = string.Format( BaseApiPath + "{0}", entity.Id );
            Service.PutData<BinaryFileType>( apiPath, entity );
        }
    }
}
