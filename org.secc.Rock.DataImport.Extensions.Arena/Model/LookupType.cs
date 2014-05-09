using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{

    [Table("core_lookup_type")]
    public partial class LookupType
    {
        public LookupType()
        {
            Lookup = new HashSet<Lookup>();
        }

        [Key]
        [Column(Order = 0)]
        public int lookup_type_id { get; set; }

        public Guid guid { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_type_name { get; set; }

        [Required]
        [StringLength(1000)]
        public string lookup_type_desc { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_category { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_1_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_2_title { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int organization_id { get; set; }

        public bool system_flag { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_3_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_4_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_5_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_6_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_7_title { get; set; }

        [Required]
        [StringLength(50)]
        public string qualifier_8_title { get; set; }

        public virtual ICollection<Lookup> Lookup { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
