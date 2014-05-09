using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_merged")]
    public partial class PersonMerged
    {
        [Key]
        public int old_person_id { get; set; }

        public int new_person_id { get; set; }

        public DateTime date_created { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        public DateTime original_date_created { get; set; }

        [Required]
        [StringLength( 50 )]
        public string original_created_by { get; set; }
    }
}
