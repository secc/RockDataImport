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

        public int? SavePersonPhoto( string fileName, string mimeType, string description, byte[] content, bool isSystem = false, bool isTemporary = false, string foreignId = null )
        {
            BinaryFile file = new BinaryFile();
            file.IsTemporary = isTemporary;
            file.IsSystem = isSystem;
            file.Url = string.Format( "~/GetImage.ashx?guid={0}", file.Guid.ToString().ToLower() );
            file.BinaryFileTypeId = (int?)(new BinaryFileTypeMap(Service).GetPhotoBinaryFileType( )).Id ?? null;
            file.FileName = fileName;
            file.MimeType = mimeType;
            file.Description = description;
            file.StorageEntityType = GetDatabaseStorageEntityType();
            file.ForeignId = foreignId;

            BinaryFileController bfController = new BinaryFileController( Service );
            bfController.Add( file );

            file = bfController.GetByGuid( file.Guid );

            if(file != null)
            {
                BinaryFileData fileData = new BinaryFileData();
                fileData.Id = file.Id;
                fileData.Content = content;

                BinaryFileDataController dataController = new BinaryFileDataController( Service );


            }




            return file.Id;
        }

        private EntityType GetDatabaseStorageEntityType()
        {
            EntityTypeController controller = new EntityTypeController( Service );

            return ( controller.GetByFilter( string.Format( "substringof('{0}', AssemblyName)", typeof( global::Rock.Storage.Provider.Database ).ToString() ) ).FirstOrDefault() );
        }

    }


}
