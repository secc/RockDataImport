using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_attribute_group")]
    public partial class ArenaAttributeGroup
    {
        public ArenaAttributeGroup()
        {
            ArenaAttribute = new HashSet<ArenaAttribute>();
        }

        [Key]
        public int attribute_group_id { get; set; }

        public Guid guid { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public int? organization_id { get; set; }

        public bool system_flag { get; set; }

        [Required]
        [StringLength( 250 )]
        public string group_name { get; set; }

        public int group_order { get; set; }

        public int display_location { get; set; }

        public virtual ICollection<ArenaAttribute> ArenaAttribute { get; set; }

        public virtual Organization Organization { get; set; }

    }
}
