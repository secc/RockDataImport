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
            throw new NotImplementedException();
        }

        public override BinaryFile GetById( int id )
        {
            throw new NotImplementedException();
        }

        public override BinaryFile GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<BinaryFile>( BaseApiPath, guid );
        }

        public override List<BinaryFile> GetAll()
        {
            throw new NotImplementedException();
        }

        public override List<BinaryFile> GetByFilter( string expression )
        {
            throw new NotImplementedException();
        }

        public override BinaryFile GetByForeignId( string foreignId )
        {
            throw new NotImplementedException();
        }

        public override void Update( BinaryFile entity )
        {
            throw new NotImplementedException();
        }
    }
}
