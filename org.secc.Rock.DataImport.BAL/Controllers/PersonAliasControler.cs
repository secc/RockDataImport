using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PersonAliasController : BaseController<PersonAlias>
    {
        string baseAPIPath = "/api/PersonAlias/";

        private PersonAliasController() : base() { }

        public PersonAliasController( RockService service ) : base( service ) { }

        public override void Add( PersonAlias entity )
        {
            Service.PostData<PersonAlias>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            Service.DeleteData( apiPath );
        }

        public override PersonAlias GetById( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            return Service.GetData<PersonAlias>( apiPath );
        }

        public override PersonAlias GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<PersonAlias>( baseAPIPath, guid );
        }

        public override List<PersonAlias> GetAll()
        {
            return Service.GetData<List<PersonAlias>>( baseAPIPath );
        }

        public override List<PersonAlias> GetByFilter( string expression )
        {
            return Service.GetData<List<PersonAlias>>( baseAPIPath, expression );
        }

        public override PersonAlias GetByForeignKey( string foreignKey )
        {
            string filter = string.Format( "ForeignKey eq '{0}'", foreignKey );
            return ( Service.GetData<List<PersonAlias>>( baseAPIPath, filter ) ).FirstOrDefault();
        }

        public override void Update( PersonAlias entity )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", entity.Id );
            Service.PutData<PersonAlias>( apiPath, entity );
        }

        public List<PersonAlias> GetByPersonId( int personId )
        {
            return GetByFilter( string.Format( "PersonId eq {0}", personId ) );
        }
    }
}
