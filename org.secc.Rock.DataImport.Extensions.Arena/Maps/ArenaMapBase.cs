using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Integration;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    public abstract class ArenaMapBase : iExportMapComponent
    {
        #region Fields
        protected int? mRecordCount;
        protected RockService Service;
        #endregion

        #region Properties
        protected Dictionary<string, string> ConnectionInfo { get; set; }
       
        public abstract int? RecordCount
        {
            get;
        }

        #endregion

        #region Constructor
        protected ArenaMapBase() { }

        protected ArenaMapBase( Dictionary<string, string> connectionInfo, RockService service )
        {
            ConnectionInfo = connectionInfo;
            Service = service;
        }

        #endregion

        public abstract List<string> GetSubsetIDs( int startingRecord, int size );

        public abstract void ExportRecord( string identifier );

        public abstract Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType );

        public Dictionary<string, Dictionary<string, object>> GetAttributes( Type parentObject, Type attributeType )
        {
            return System.Attribute.GetCustomAttributes( parentObject )
            .Where( a => a.GetType() == attributeType )
            .Select( a => new
            {
                Name = a.GetType().GetProperties().Where( p => p.Name == "Name" ).Select( p => p.GetValue( a ) ).FirstOrDefault().ToString(),
                Attribute = a.GetType().GetProperties().ToDictionary( p => p.Name, p1 => p1.GetValue( a ) )
            } ).ToDictionary( a => a.Name, a => a.Attribute );

        }



        public abstract event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;


    }
}
