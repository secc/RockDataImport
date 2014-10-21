using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_attribute")]
    public class ArenaAttribute
    {
        public ArenaAttribute()
        {
            PersonAttribute = new HashSet<PersonAttribute>();
            PersonAttributeHistory = new HashSet<PersonAttributeHistory>();
        }

        [Key]
        [Column(Order = 0)]
        public int attribute_id { get; set; }

        public Guid guid { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength(50)]
        public string created_by { get; set; }

        [Required]
        [StringLength(50)]
        public string modified_by { get; set; }

        public int? attribute_group_id { get; set; }

        [Required]
        public string attribute_name { get; set; }

        public int attribute_type { get; set; }

        public int attribute_order { get; set; }

        public bool visible { get; set; }

        public bool required { get; set; }

        [Required]
        [StringLength(100)]
        public string type_qualifier { get; set; }

        [Column("readonly")]
        public bool _readonly { get; set; }

        public bool system_flag { get; set; }

        public bool history_enabled { get; set; }

        public int max_history_entries { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int organization_id { get; set; }

        public virtual ArenaAttributeGroup ArenaAttributeGroup { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<PersonAttribute> PersonAttribute { get; set; }

        public virtual ICollection<PersonAttributeHistory> PersonAttributeHistory { get; set; }
    }
 
}
