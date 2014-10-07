using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Helper;
using org.secc.Rock.DataImport.BAL.Integration;
using org.secc.Rock.DataImport.BAL.RockMaps;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Person Previous Name")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "People last name records for ArenaChMS. This requires that the Person records to have previously been loaded.")]
    [Dependency("Person", typeof(PersonMap), 0)]

    public class PersonPreviousNameMap : ArenaMapBase
    {

        #region Fields

        #endregion

        #region Properties
        #endregion

        #region Constructors

        private PersonPreviousNameMap() : base( typeof( PersonPreviousNameMap ) ) { }

        [ImportingConstructor]
        public PersonPreviousNameMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo, [Import( "RockService" )] RockService service ) : base( typeof( PersonMap ), connectionInfo, service ) { }

        #endregion

        #region Public Methods
        public override int? RecordCount
        {
            get { return GetRecordCount(); }
        }

        public override List<string> GetSubsetIDs( int startingRecord, int size )
        {
            throw new NotImplementedException();
        }

        public override void ExportRecord( string identifier )
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType )
        {
            return GetAttributes( this.GetType(), attributeType );
        }
        #endregion

        public override event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        #region Private Methods

        private int? GetRecordCount()
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.PersonPreviousName.Select( n => n.person_id ).Distinct().Count();
            }
        }

        #endregion
    }
}
