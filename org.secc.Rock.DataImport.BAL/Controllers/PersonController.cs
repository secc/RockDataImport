﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public class PersonController :BaseController<Person>
    {
        string baseAPIPath = "/api/People/";

        private PersonController() { }

        public PersonController( RockService service )
        {
            Service = service;
        }

        public override void Add( Person entity )
        {
            Service.PostData<Person>( baseAPIPath, entity );
        }

        public override void Delete( int id )
        {
            Service.DeleteData( string.Format( baseAPIPath + "{0}", id ) );
        }

        public override Person GetById( int id )
        {
            string apiPath = string.Format( baseAPIPath + "{0}", id );
            return Service.GetData<Person>( apiPath );
        }

        public override Person GetByGuid( Guid guid )
        {
            return Service.GetDataByGuid<Person>( baseAPIPath, guid );
        }

        public override List<Person> GetAll()
        {
            return Service.GetData<List<Person>>( baseAPIPath );
        }

        public override List<Person> GetByFilter( string expression )
        {
            return Service.GetData<List<Person>>( baseAPIPath, expression );
        }

        public override Person GetByForeignKey( string foreignKey )
        {
            string expression = string.Format( "ForeignKey eq {0}", foreignKey );
            return (GetByFilter( expression )).FirstOrDefault();
        }

        public override void Update( Person entity )
        {
            string apiPath = string.Format( "api/People/{0}", entity.Id );
            Service.PutData<Person>( apiPath, entity );
        }

        public Person GetByUserName( string userName )
        {
            string apiMethod = string.Format( baseAPIPath + "GetByUserName/{0}", userName );

            return Service.GetData<Person>( apiMethod );
        }
    }
}
