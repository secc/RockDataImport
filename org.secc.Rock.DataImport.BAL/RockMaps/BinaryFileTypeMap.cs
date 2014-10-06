using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

using System.Text;
using System.Threading.Tasks;

using org.secc.Rock.DataImport.BAL.Controllers;
using Rock.Model;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class BinaryFileTypeMap
    {
        #region Fields
        public static string PhotoFiletypeGuid = "03BD8476-8A9F-4078-B628-5B538F967AFC";
        #endregion

        #region Properties
        RockService Service;
        #endregion

        #region Constructors
        private BinaryFileTypeMap() { }

        public BinaryFileTypeMap( RockService service )
        {
            Service = service;
        }
        #endregion

        public bool IsPhotoUploadEnabled( )
        {
            bool usesDatabaseFileStorage = false;
            const string CACHE_KEY = "EnablePhotoUpload";

            ObjectCache cache = MemoryCache.Default;

            if ( cache.Contains( CACHE_KEY ) )
            {
                return (bool)cache[CACHE_KEY];
            }

            BinaryFileType fileType = GetPhotoBinaryFileType( );
            
            if ( fileType.StorageEntityTypeId != null )
            {
                EntityTypeController entityTypeController = new EntityTypeController( Service );
                fileType.StorageEntityType = entityTypeController.GetById( (int)fileType.StorageEntityTypeId );
            }

            if ( fileType.StorageEntityType != null && fileType.StorageEntityType.AssemblyName.Contains( typeof( global::Rock.Storage.Provider.Database ).ToString() ) )
            {
                usesDatabaseFileStorage = true;
            }

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            cache.Set( CACHE_KEY, usesDatabaseFileStorage, policy );

            return usesDatabaseFileStorage;
            
        }

        public Dictionary<string,object> GetPhotoBinaryFileTypeDictionary()
        {
            return GetPhotoBinaryFileType().ToDictionary();
        }

        internal BinaryFileType GetPhotoBinaryFileType()
        {
            const string CACHE_KEY = "PhotoFileType";

            ObjectCache cache = MemoryCache.Default;

            if ( cache.Contains( CACHE_KEY ) )
            {
                return ( (BinaryFileType)cache[CACHE_KEY] );
            }

            BinaryFileTypeController controller = new BinaryFileTypeController( Service );
            var fileType = controller.GetByGuid( new Guid( PhotoFiletypeGuid ) );

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan( 0, 5, 0 );

            cache.Set( CACHE_KEY, fileType, policy );

            return fileType;
        }

    }
}
