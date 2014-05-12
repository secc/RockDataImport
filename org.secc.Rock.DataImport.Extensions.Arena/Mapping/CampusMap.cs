using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Integration;

namespace org.secc.Rock.DataImport.Extensions.Arena.Mapping
{
    [Export(ArenaIntegration.IDENTIFIER, typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Campus")]
    public class CampusMap : iExportMapComponent
    {
        private int? mRecordCount;

        //public int? RecordCount
        //{
        //    get
        //    {
        //        if ( mRecordCount == null )
        //        {
        //            mRecordCount = GetRecordCount();
        //        }

        //        return mRecordCount;
        //    }
        //}

        public List<string> GetSubsetIDs( Dictionary<string,string> connection,  int startingRecord = 0, int size = 0 )
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( connection ) )
            {
                var query = Context.Campus
                            .Skip( startingRecord );

                if ( size > 0 )
                {
                    query = query.Take( size );
                }

                return query.Select( c => c.campus_id.ToString() ).ToList();
            }
        }

        public void ExportRecord( Dictionary<string,string> connection, string identifier )
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> OnExportSuccess;

        public event EventHandler<EventArgs> OnExportFailure;


        public int? GetRecordCount(Dictionary<string,string> connection)
        {
            using ( Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext( connection ) )
            {
                return Context.Campus.Count();
            }
        }
    }
}
