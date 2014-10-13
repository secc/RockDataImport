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
    [ExportMetadata("Description", "Maps a campus leader from Arena to their Campus in Rock. Separate integration is used because of the relationships that exists between Person and Campus.")]
    [ExportMetadata("ImportRanking", 0)]
    [Dependency("Person", typeof(PersonMap))]
    [Dependency("Campus", typeof(CampusMap))]
    
    public class CampusLeaderMap : ArenaMapBase
    {
        public override int? RecordCount
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


        private CampusLeaderMap() : base( typeof( CampusLeaderMap ) ) { }

        [ImportingConstructor]
        public CampusLeaderMap( [Import( "ConnectionInfo" )] Dictionary<string, string> connectionInfo, [Import("RockService")] RockService service ) : base( typeof( CampusLeaderMap ), connectionInfo, service ) { }

        public override List<string> GetSubsetIDs( int startingRecord, int size )
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

        public override void ExportRecord( string identifier )
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

            Dictionary<string, object> rockCampus = GetRockCampusByArenaId( arenaCampusId );

            if ( rockCampus == null )
            {
                var campusTask = SaveCampus( arenaCampusId );

                if ( campusTask.Result != null )
                {
                    rockCampus = GetRockCampus( (int)campusTask.Result );
                }
                else
                {
                    OnExportAttemptCompleted( identifier, false );
                    return;
                }
            }

            Dictionary<string, object> campusLeader = GetRockCampusLeaderByArenaId( (int) arenaCampus.leader_person_id );

            if ( campusLeader == null )
            {
                var personTask = SaveLeader( (int)arenaCampus.leader_person_id );

                if ( personTask.Result != null )
                {
                    campusLeader = GetRockCampusLeader( (int)personTask.Result );

                    if ( campusLeader == null )
                    {
                        OnExportAttemptCompleted( identifier, false );
                        return;
                    }
                }
            }

            RockMap.CampusMap campusMap = new RockMap.CampusMap( Service );
            bool isSystem = (bool)rockCampus["IsSystem"];
            string name = (string)rockCampus["Name"];
            string shortCode = (string)rockCampus["ShortCode"];
            int? locationId = (int?)rockCampus["LocationId"];
            Guid guid = (Guid)rockCampus["Guid"];
            string foreignId = (string)rockCampus["ForeignId"];
            string phoneNumber = (string)rockCampus["PhoneNumber"];
            string serviceTimes = (string)rockCampus["ServiceTimes"];
            string description = (string)rockCampus["Description"];
            bool isActive = (bool)rockCampus["IsActive"];
            string url = (string)rockCampus["Url"];
            int leaderPersonAliasId = (int)campusLeader["PrimaryAliasId"];


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

        public override Dictionary<string, Dictionary<string, object>> GetAttributes( Type attributeType )
        {
            return GetAttributes( this.GetType(), attributeType );
        }

        public override event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

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

        private Dictionary<string, object> GetRockCampus( int rockCampusId )
        {
            RockMap.CampusMap campusMap = new RockMap.CampusMap( Service );
            return campusMap.GetById( rockCampusId );
        }

        private Dictionary<string, object> GetRockCampusByArenaId( int arenaCampusId )
        {
            RockMap.CampusMap campusMap = new RockMap.CampusMap( Service );

            return campusMap.GetByForeignId( arenaCampusId.ToString() );

        }

        private Dictionary<string, object> GetRockCampusLeader( int personId )
        {
            RockMap.PersonMap personMap = new RockMap.PersonMap( Service );
            return personMap.GetById( personId, true );
        }

        private Dictionary<string, object> GetRockCampusLeaderByArenaId( int arenaLeaderPersonId )
        {
            RockMap.PersonMap personMap = new RockMap.PersonMap( Service );
            return personMap.GetByForeignId( arenaLeaderPersonId.ToString(), true );
        }

        public virtual void OnExportAttemptCompleted( string identifier, bool isSuccess, int? rockId = null, Type mapType = null )
        {
            ExportMapEventArgs args = new ExportMapEventArgs();

            if ( mapType == null )
            {
                args.MapType = this.GetType();
            }
            else
            {
                args.MapType = mapType; 
            }

            args.Identifier = identifier;
            args.RockIdentifier = rockId;
            args.IsSuccess = isSuccess;

            EventHandler<ExportMapEventArgs> handler = ExportAttemptCompleted;

            if ( handler != null )
            {
                handler( this, args );
            }
        }

        private Task<int ?> SaveCampus(int arenaCampusId )
        {
            var tcs = new TaskCompletionSource<int?>();
            var CampusMap = new CampusMap( ConnectionInfo, Service );

            CampusMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );

            CampusMap.ExportRecord( arenaCampusId.ToString() );

            return tcs.Task;

        }

        private Task<int?> SaveLeader( int arenaLeaderPersonId )
        {
            var tcs = new TaskCompletionSource<int?>();
            var PersonMap = new PersonMap( ConnectionInfo, Service );

            PersonMap.ExportAttemptCompleted += ( o, e ) => tcs.SetResult( e.RockIdentifier );
            PersonMap.ExportRecord( arenaLeaderPersonId.ToString() );

            return tcs.Task;
        }
    }
}
