using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_email")]
    public partial class PersonEmail
    {
        [Key]
        public int email_id { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public int person_id { get; set; }

        public bool active { get; set; }

        public int email_order { get; set; }

        [Required]
        [StringLength( 80 )]
        public string email { get; set; }

        [Required]
        [StringLength( 500 )]
        public string notes { get; set; }

        public bool allow_bulk_mail { get; set; }

        public virtual Person Person { get; set; }
    }
}
