﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Integration;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Campus")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "Campus locations from ArenaChMS.")]
    public class CampusMap : iExportMapComponent
    {
        private int? mRecordCount;

        private Dictionary<string,string> ConnectionInfo{get;set;}

        public int? RecordCount
        {
            get
            {
                if ( mRecordCount == null )
                {
                    mRecordCount = GetRecordCount();
                }

                return mRecordCount;
            }
        }

        private CampusMap() { }

        [ImportingConstructor]
        public CampusMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo )
        {
            ConnectionInfo = connectionInfo;
        }

        public List<string> GetSubsetIDs( int startingRecord = 0, int size = 0 )
        {
            using ( Model.ArenaContext Context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
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

        public void ExportRecord( string identifier, RockService service  )
        {
            int campusId = 0;
            if ( !int.TryParse( identifier, out campusId ) )
            {
                OnExportAttemptCompleted( identifier, false );
                return;
            }

            Model.Campus arenaCampus = GetArenaCampus( campusId );

            if ( arenaCampus == null )
            {
                OnExportAttemptCompleted( identifier, false );
                return; 
            }

            BAL.RockMaps.CampusMap rockCampusMap = new BAL.RockMaps.CampusMap( service );
            Dictionary<string, object> rockCampus = rockCampusMap.GetByForeignKey( campusId.ToString() );

            if ( rockCampus == null )
            {
                int? rockCampusId = rockCampusMap.SaveCampus( isSystem: false, name: arenaCampus.name, foreignKey: arenaCampus.campus_id.ToString() );
                if ( rockCampusId == null )
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }
            }

            OnExportAttemptCompleted( identifier, true );        

        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if(handler != null)
            {
                handler( this, args );
            }
        }

        public event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;


        private Model.Campus GetArenaCampus( int campusId )
        {
            using ( Model.ArenaContext context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.Campus.FirstOrDefault( c => c.campus_id == campusId );
            }
        }

        private int? GetRecordCount()
        {
            using ( Model.ArenaContext Context = Arena.Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return Context.Campus.Count();
            }
        }
    }
}
