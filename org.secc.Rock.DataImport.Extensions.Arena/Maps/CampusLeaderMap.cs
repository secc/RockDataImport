using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Integration;
using RockMap = org.secc.Rock.DataImport.BAL.RockMaps;
using org.secc.Rock.DataImport.Extensions.Arena.Model;

namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    [Export(typeof(iExportMapComponent))]
    [ExportMetadata("Name", "Campus Leader")]
    [ExportMetadata("Integration", ArenaIntegration.IDENTIFIER)]
    [ExportMetadata("Description", "Maps a campus leader from Arena to their Campus in Rock. Separate integration is used because of the dependencies that exist between Campus and Person.")]
    [Dependency("Campus", typeof(CampusMap), 0)]
    [Dependency("Person", typeof(PersonMap), 1)]
    
    public class CampusLeaderMap : iExportMapComponent
    {
        private int? mRecordCount;
        private int? mDefinedTypeCount;

        private Dictionary<string, string> ConnectionInfo { get; set; }
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

        public int? DefinedTypeCount
        {
            get { return mDefinedTypeCount; }
        }

        private CampusLeaderMap() { }

        [ImportingConstructor]
        public CampusLeaderMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo )
        {
            ConnectionInfo = connectionInfo;
        }

        public List<string> GetSubsetIDs( int startingRecord, int size )
        {
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                var query = context.Campus.Where( c => c.leader_person_id != null )
                                .OrderBy( c => c.campus_id )
                                .Select( c => c.campus_id.ToString() )
                                .Skip( startingRecord );
                if ( size > 0 )
                {
                    query = query.Take( size );
                }

                return query.ToList();
            }
        }

        public void ExportRecord( string identifier, RockService service )
        {
            int arenaCampusId = 0;

            if ( !int.TryParse( identifier, out arenaCampusId ) )
            {
                OnExportAttemptCompleted( identifier, false );
                return;
            }

            Campus arenaCampus = GetArenaCampus( arenaCampusId );

            if(arenaCampus == null)
            {
                OnExportAttemptCompleted(identifier, false);
                return;
            }

            if(arenaCampus.leader_person_id == null)
            {
                OnExportAttemptCompleted( identifier, true );
                return;

            }

            Dictionary<string, object> rockCampus = GetRockCampusByArenaId( arenaCampusId, service );

            if ( rockCampus == null )
            {
                var campusTask = SaveCampus( arenaCampusId, service );

                if ( campusTask.Result != null )
                {
                    rockCampus = GetRockCampus( (int)campusTask.Result, service );
                }
                else
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }
            }

            Dictionary<string, object> campusLeader = GetRockCampusLeaderByArenaId( (int) arenaCampus.leader_person_id, service );

            if ( campusLeader == null )
            {
                var personTask = SaveLeader( (int)arenaCampus.leader_person_id, service );

                if ( personTask.Result != null )
                {
                    campusLeader = GetRockCampusLeader( (int)personTask.Result, service );

                    if ( campusLeader == null )
                    {
                        OnExportAttemptCompleted( identifier, false );
                        return;
                    }
                }
            }

            RockMap.CampusMap campusMap = new RockMap.CampusMap( service );
            bool isSystem = (bool)rockCampus["IsSystem"];
            string name = rockCampus["Name"].ToString();
            string shortCode = rockCampus["ShortCode"].ToString();
            int? locationId = (int?)rockCampus["LocationId"];
            Guid guid = (Guid)rockCampus["Guid"];
            string foreignId = rockCampus["ForeignId"].ToString();
            string phoneNumber = rockCampus["PhoneNumber"].ToString();
            string serviceTimes = rockCampus["ServiceTimes"].ToString();
            string description = rockCampus["Description"].ToString();
            bool isActive = (bool)rockCampus["IsActive"];
            string url = rockCampus["Url"].ToString();
            int leaderPersonAliasId = (int)campusLeader["Id"];

            int? campusId = campusMap.SaveCampus( isSystem, name, shortCode, locationId, phoneNumber, leaderPersonAliasId, serviceTimes, foreignId, (int)rockCampus["Id"] );

            if ( campusId != null )
            {
                OnExportAttemptCompleted( identifier, true, campusId );

            }
            else
            {
                OnExportAttemptCompleted( identifier, false );
            }
        }

        public event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        private Campus GetArenaCampus( int campusId )
        {
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.Campus.FirstOrDefault( c => c.campus_id == campusId );
            }
        }

        private int? GetRecordCount()
        {
            using ( ArenaContext context = ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.Campus
                    .Where( c => c.leader_person_id != null ).Count();
            }
        }

        private Dictionary<string, object> GetRockCampus( int rockCampusId, RockService service )
        {
            RockMap.CampusMap campusMap = new RockMap.CampusMap( service );
            return campusMap.GetById( rockCampusId );
        }

        private Dictionary<string, object> GetRockCampusByArenaId( int arenaCampusId, RockService service )
        {
            RockMap.CampusMap campusMap = new RockMap.CampusMap( service );

            return campusMap.GetByForeignId( arenaCampusId.ToString() );

        }

        private Dictionary<string, object> GetRockCampusLeader( int personId, RockService service )
        {
            RockMap.PersonMap personMap = new RockMap.PersonMap( service );
            return personMap.GetById( personId );
        }

        private Dictionary<string, object> GetRockCampusLeaderByArenaId( int arenaLeaderPersonId, RockService service )
        {
            RockMap.PersonMap personMap = new RockMap.PersonMap( service );
            return personMap.GetByForeignId( arenaLeaderPersonId.ToString() );
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();
            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if ( handler != null )
            {
                handler( this, args );
            }
        }

        private Task<int ?> SaveCampus(int arenaCampusId, RockService service)
        {
            var tcs = new TaskCompletionSource<int?>();
            var CampusMap = new CampusMap( ConnectionInfo );

            CampusMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );

            CampusMap.ExportRecord( arenaCampusId.ToString(), service );

            return tcs.Task;

        }

        private Task<int?> SaveLeader( int arenaLeaderPersonId, RockService service )
        {
            var tcs = new TaskCompletionSource<int?>();
            var PersonMap = new PersonMap( ConnectionInfo );

            PersonMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );
            PersonMap.ExportRecord( arenaLeaderPersonId.ToString(), service );

            return tcs.Task;
        }
    }
}
