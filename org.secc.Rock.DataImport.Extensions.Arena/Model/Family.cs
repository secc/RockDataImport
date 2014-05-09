using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_family")]
    public partial class Family
    {
        public Family()
        {
            FamilyMember = new HashSet<FamilyMember>();
        }

        [Key]
        public int family_id { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength(50)]
        public string created_by { get; set; }

        [Required]
        [StringLength(50)]
        public string modified_by { get; set; }

        [Required]
        [StringLength(100)]
        public string family_name { get; set; }

        public int? foreign_key { get; set; }

        public int organization_id { get; set; }

        public virtual ICollection<FamilyMember> FamilyMember { get; set; }

        public virtual Organization Organization { get; set; }

    }
}
