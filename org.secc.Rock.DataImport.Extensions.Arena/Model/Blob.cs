using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("util_blob")]
    public partial class Blob
    {
        public Blob()
        {
            BlobOrganizations = new HashSet<Organization>();
            BlobPersons = new HashSet<Person>();
        }

        [Key]
        public int blob_id { get; set; }

        public Guid guid { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        [Required]
        [StringLength( 20 )]
        public string file_ext { get; set; }

        [Required]
        [StringLength( 100 )]
        public string mime_type { get; set; }

        public byte[] blob { get; set; }

        [Required]
        [StringLength( 100 )]
        public string original_file_name { get; set; }

        [Required]
        [StringLength( 100 )]
        public string title { get; set; }

        [Required]
        [StringLength( 255 )]
        public string description { get; set; }

        public int? document_type_id { get; set; }

        public int? organization_id { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Organization> BlobOrganizations { get; set; }

        public virtual ICollection<Person> BlobPersons { get; set; }
    }
}
