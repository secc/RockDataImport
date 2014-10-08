using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;
namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class FieldTypeController : BaseController<FieldType>
    {
        string BaseApiPath = "/api/FieldTypes/";

        #region Controllers
        private FieldTypeController() : base() { }

        public FieldTypeController( RockService service ) : base( service ) { }

        #endregion

        #region Public

        public override void Add( FieldType entity )
        {
            Service.PutData<FieldType>( BaseApiPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            Service.DeleteData( apiPath );
        }

        public override FieldType GetById( int id )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, id );
            return Service.GetData<FieldType>( apiPath );
        }

        public override FieldType GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<FieldType>( BaseApiPath, guid );
        }

        public override List<FieldType> GetAll()
        {
            return Service.GetData<List<FieldType>>( BaseApiPath );
        }

        public override List<FieldType> GetByFilter( string expression )
        {
            return Service.GetData<List<FieldType>>( BaseApiPath, expression );
        }

        public override FieldType GetByForeignId( string foreignId )
        {
            string expression = string.Format( "ForeignId eq '{0}'", foreignId );
            return GetByFilter( expression ).FirstOrDefault();

        }

        public override void Update( FieldType entity )
        {
            string apiPath = string.Format( "{0}{1}", BaseApiPath, entity.Id );
            Service.PutData<FieldType>( apiPath, entity );
        }

        public FieldType GetByClassName( string className )
        {
            string expression = string.Format( "Class eq '{0}'", className );
            return GetByFilter( expression ).FirstOrDefault();
        }


        #endregion


    }
}
