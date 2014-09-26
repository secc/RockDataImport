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
        string BaseApiPath = "/api/BaseFileDatas";

        private BinaryFileDataController() : base() { }

        public BinaryFileDataController( RockService service ) : base( service ) { }

        public override void Add( BinaryFileData entity )
        {
            Service.PostData<BinaryFileData>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            throw new NotImplementedException();
        }

        public override BinaryFileData GetById( int id )
        {
            throw new NotImplementedException();
        }

        public override BinaryFileData GetByGuid( Guid guid )
        {
            throw new NotImplementedException();
        }

        public override List<BinaryFileData> GetAll()
        {
            throw new NotImplementedException();
        }

        public override List<BinaryFileData> GetByFilter( string expression )
        {
            throw new NotImplementedException();
        }

        public override BinaryFileData GetByForeignId( string foreignId )
        {
            throw new NotImplementedException();
        }

        public override void Update( BinaryFileData entity )
        {
            throw new NotImplementedException();
        }
    }
}
