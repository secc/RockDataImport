using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;


namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class BinaryFileController : BaseController<BinaryFile>
    {
        string BaseApiPath = "api/BinaryFiles/";

        private BinaryFileController() : base() { }

        public BinaryFileController( RockService service ) : base( service ) { }


        public override void Add( BinaryFile entity )
        {
            Service.PostData<BinaryFile>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            Service.DeleteData( apiPath );

        }

        public override BinaryFile GetById( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            return Service.GetData<BinaryFile>( apiPath );
        }

        public override BinaryFile GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<BinaryFile>( BaseApiPath, guid );
        }

        public override List<BinaryFile> GetAll()
        {
            return Service.GetData<List<BinaryFile>>( BaseApiPath );
        }

        public override List<BinaryFile> GetByFilter( string expression )
        {
            return Service.GetData<List<BinaryFile>>( BaseApiPath, expression );
        }

        public override BinaryFile GetByForeignId( string foreignId )
        {
            string expression = String.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();
        }

        public override void Update( BinaryFile entity )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, entity.Id );
            Service.PutData<BinaryFile>( apiPath, entity );
        }
    }
}
