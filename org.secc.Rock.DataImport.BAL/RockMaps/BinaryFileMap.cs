using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;
using org.secc.Rock.DataImport.BAL.Controllers;

namespace org.secc.Rock.DataImport.BAL.RockMaps
{
    public class BinaryFileMap : MapBase
    {
        RockService Service { get; set; }

        private BinaryFileMap() { }

        public BinaryFileMap( RockService service )
        {
            Service = service;
        }

        public Dictionary<string, object> GetByForeignId( string foreignId )
        {
            BinaryFileController fileController = new BinaryFileController( Service );
            BinaryFile file = fileController.GetByForeignId( foreignId );

            if ( file != null && file.StorageEntityTypeId == GetDatabaseStorageEntityType().Id )
            {
                BinaryFileDataController dataController = new BinaryFileDataController( Service );
                file.Data = dataController.GetById( file.Id );
            }

            var fileDictionary = ToDictionary( file );

            if ( fileDictionary != null )
            {
                fileDictionary.Add( "Data", ToDictionary( file.Data ) );
            }

            return fileDictionary;
        }

        public int? SavePersonPhoto( string fileName, string mimeType, string description, byte[] content, bool isSystem = false, bool isTemporary = false, string foreignId = null )
        {
            BinaryFile binaryFile = new BinaryFile();
            binaryFile.FileName = fileName;
            binaryFile.BinaryFileTypeId = new BinaryFileTypeMap(Service).GetPhotoBinaryFileType().Id;
            binaryFile.Url = string.Format( "~/GetImage.ashx?guid={0}", binaryFile.Guid );
            binaryFile.IsSystem = false;
            binaryFile.MimeType = mimeType;
            binaryFile.Description = description;
            binaryFile.IsTemporary = isTemporary;
            binaryFile.ForeignId = foreignId;

            BinaryFileController fileController = new BinaryFileController( Service );
            fileController.Add( binaryFile );

            binaryFile = fileController.GetByGuid(binaryFile.Guid);

            binaryFile.Data = new BinaryFileData();
            binaryFile.Data.Content = content;
            binaryFile.Data.Id = binaryFile.Id;

            BinaryFileDataController fileDataController = new BinaryFileDataController( Service );
            fileDataController.Add( binaryFile.Data );

            return binaryFile.Id;



            
        }

        private EntityType GetDatabaseStorageEntityType()
        {
            EntityTypeController controller = new EntityTypeController( Service );

            return ( controller.GetByFilter( string.Format( "substringof('{0}', AssemblyName)", typeof( global::Rock.Storage.Provider.Database ).ToString() ) ).FirstOrDefault() );
        }

    }


}
