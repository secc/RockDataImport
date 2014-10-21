using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_history")]
    public partial class PersonHistory
    {
        [Key]
        public int person_history_id { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public int person_id { get; set; }

        public int history_type_luid { get; set; }

        public int history_qualifier_id { get; set; }

        public bool system_history { get; set; }

        [Column( TypeName = "ntext" )]
        public string history { get; set; }

        public int organization_id { get; set; }

        public bool display_flag { get; set; }

        public DateTime display_expiration { get; set; }

        public bool private_flag { get; set; }

        public virtual Lookup HistoryType { get; set; }

        public virtual Person Person { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
