using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace org.secc.Rock.DataImport.BAL.Helper
{
    public class DefinedValueMatch
    {
        static string DEFINED_VALUE_FILE_NAME = "DefinedValues.xml";

        public string SourceDefinedTypeId { get; set; }
        public string SourceDefinedValueId { get; set; }
        public int RockDefinedTypeId { get; set; }
        public int RockDefinedValueId { get; set; }


        public void Save()
        {
            List<DefinedValueMatch> definedValues = GetMatchedDefinedValues();

            DefinedValueMatch matchedDefinedValue = definedValues.Where( dv => dv.SourceDefinedTypeId == SourceDefinedTypeId && dv.SourceDefinedValueId == SourceDefinedValueId ).FirstOrDefault();

            if ( matchedDefinedValue == null )
            {
                definedValues.Add( this );
            }
            else
            {
                matchedDefinedValue.RockDefinedTypeId = RockDefinedTypeId;
                matchedDefinedValue.RockDefinedValueId = RockDefinedValueId;
            }

            XmlSerializer serializer = new XmlSerializer( typeof( List<DefinedValueMatch> ) );

            using ( StreamWriter sw = new StreamWriter( GetDefinedValueFilePath(), false ) )
            {
                serializer.Serialize( sw, definedValues );
            }
        }

        public static List<DefinedValueMatch> GetMatchedDefinedValues()
        {
            ObjectCache cache = MemoryCache.Default;
            const string definedValueCacheKey = "DefinedValueCache";

            List<DefinedValueMatch> definedValues = new List<DefinedValueMatch>();

            definedValues.AddRange( cache[definedValueCacheKey] as List<DefinedValueMatch> );

            if ( definedValues.Count == 0 )
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );
                policy.ChangeMonitors.Add( new HostFileChangeMonitor( new List<string>() { GetDefinedValueFilePath() } ) );

                if ( File.Exists( GetDefinedValueFilePath() ) )
                {
                    XmlSerializer serializer = new XmlSerializer( typeof( List<DefinedValueMatch> ) );
                    using ( StreamReader sr = new StreamReader( GetDefinedValueFilePath() ) )
                    {
                        definedValues.AddRange( serializer.Deserialize( sr ) as List<DefinedValueMatch> );
                    }
                    cache.Set( definedValueCacheKey, definedValues, policy );
                }
            }

            return definedValues;
        }

        public static string GetDefinedValueFilePath()
        {
            return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, DEFINED_VALUE_FILE_NAME );
        }

        public static int? GetDefinedValueMatch( string sourceId )
        {
            return GetMatchedDefinedValues().FirstOrDefault( dv => dv.SourceDefinedValueId == sourceId ).RockDefinedValueId;
        }
    }

}
