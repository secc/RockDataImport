using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_attribute_history")]
    public partial class PersonAttributeHistory
    {
        [Key]
        public int history_id { get; set; }

        public int person_id { get; set; }

        public int attribute_id { get; set; }

        public int? int_value { get; set; }

        public string varchar_value { get; set; }

        public DateTime? datetime_value { get; set; }

        public decimal? decimal_value { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public int organization_id { get; set; }

        public virtual ArenaAttribute ArenaAttribute { get; set; }

        public virtual Person Person { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
