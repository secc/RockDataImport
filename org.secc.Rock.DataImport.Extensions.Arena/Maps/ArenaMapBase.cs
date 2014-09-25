using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL;
using org.secc.Rock.DataImport.BAL.Attribute;
using org.secc.Rock.DataImport.BAL.Helper;
using org.secc.Rock.DataImport.BAL.Integration;
using org.secc.Rock.DataImport.BAL.RockMaps;


namespace org.secc.Rock.DataImport.Extensions.Arena.Maps
{
    public abstract class ArenaMapBase : iExportMapComponent
    {
        #region Fields
        protected int? mRecordCount;
        private Type MapType;
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
        protected ArenaMapBase(Type mapType) 
        {
            MapType = mapType;
        }

        protected ArenaMapBase(Type mapType, Dictionary<string, string> connectionInfo, RockService service )
        {
            MapType = mapType;
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

        private Model.LookupType GetArenaLookupType( string arenaLTGuidString )
        {
            Guid ltGuid = Guid.NewGuid();

            if ( !Guid.TryParse( arenaLTGuidString, out ltGuid ) )
            {
                return null;
            }

            using ( Model.ArenaContext context = Model.ArenaContext.BuildContext( ConnectionInfo ) )
            {
                return context.LookupType.FirstOrDefault( lt => lt.guid == ltGuid );
            }
        }

        public List<DefinedTypeSummary> GetDefinedTypes( bool resetCache = false )
        {
            string definedTypeCacheKey = string.Format( "{0}_DefinedTypeCache", MapType.ToString() );
            ObjectCache cache = MemoryCache.Default;
            List<DefinedTypeSummary> definedTypes = new List<DefinedTypeSummary>();


            if ( cache.Contains( definedTypeCacheKey ) && !resetCache )
            {
                 definedTypes.AddRange( cache[definedTypeCacheKey] as List<DefinedTypeSummary> );
            }

            if ( definedTypes.Count == 0 )
            {
                foreach ( var mappedDT in GetAttributes(MapType, typeof(DefinedTypeAttribute) ) )
                {
                    DefinedTypeMap dtm = new DefinedTypeMap( Service );
                    Guid rockDTGuid = Guid.NewGuid();


                    if ( Guid.TryParse( (string)mappedDT.Value["RockDefinedTypeGuid"], out rockDTGuid ) )
                    {
                        definedTypes.Add( dtm.GetDefinedTypeSummary( rockDTGuid ) );
                    }
                    else
                    {
                        var arenaLookupType = GetArenaLookupType( (string)mappedDT.Value["SourceDefinedTypeIdentifier"] );

                        if ( arenaLookupType != null )
                        {
                            definedTypes.Add( dtm.GetDefinedTypeSummaryByForeignId( arenaLookupType.lookup_type_id.ToString() ) );
                        }
                    }
                }

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new TimeSpan( 0, 1, 0 );

                cache.Set( definedTypeCacheKey, definedTypes, policy );
            }

            return definedTypes;
        }
       
        public abstract event EventHandler<ExportMapEventArgs> ExportAttemptCompleted;

        protected DefinedValueSummary GetDefinedValueByForeignId( string foreignId )
        {
            return GetDefinedTypes()
                .SelectMany( dt => dt.ValueSummaries )
                .Where( dv => dv.ForeignIdValues.Contains( foreignId ) )
                .FirstOrDefault();
        }

        protected int? GetDefinedValueIdByForeignId( int? lookupId )
        {
            int? dvId = null;

            if ( lookupId == null )
            {
                return dvId;
            }

            var definedValue = GetDefinedValueByForeignId( lookupId.ToString() );

            if ( definedValue != null )
            {
                dvId = int.Parse( definedValue.Id );
            }

            return dvId;
        }

    }
}
