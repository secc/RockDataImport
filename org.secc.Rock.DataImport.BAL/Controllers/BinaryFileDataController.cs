using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class BinaryFileDataController : BaseController<BinaryFileData>
    {
        string BaseApiPath = "/api/BinaryFileDatas";

        private BinaryFileDataController() : base() { }

        public BinaryFileDataController( RockService service ) : base( service ) { }

        public override void Add( BinaryFileData entity )
        {
            Service.PostData<BinaryFileData>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            Service.DeleteData( apiPath );
        }

        public override BinaryFileData GetById( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            return Service.GetData<BinaryFileData>( apiPath );
        }

        public override BinaryFileData GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<BinaryFileData>( BaseApiPath, guid );
        }

        public override List<BinaryFileData> GetAll()
        {
            return Service.GetData<List<BinaryFileData>>( BaseApiPath );
        }

        public override List<BinaryFileData> GetByFilter( string expression )
        {
            return Service.GetData<List<BinaryFileData>>( BaseApiPath, expression );
        }

        public override BinaryFileData GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( BinaryFileData entity )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, entity.Id );
            Service.PutData<BinaryFileData>( apiPath, entity );
        }
    }
}
