using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.secc.Rock.DataImport.BAL.Controllers
{
    public abstract class BaseController<T>
    {
        public RockService Service { get; set; }

        protected BaseController() { }

        public BaseController( RockService service )
        {
            Service = service;
        }

        public abstract void Add( T entity );
        public abstract void Delete( int id );
        public abstract T GetById( int id );
        public abstract T GetByGuid( Guid guid );
        public abstract List<T> GetAll();
        public abstract List<T> GetByFilter(string expression);
        public abstract T GetByForeignId( string foreignId );
        public abstract void Update( T entity );
    }
}
