using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_family_member")]
    public partial class FamilyMember
    {
        [Key]
        [Column( Order = 0 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int family_id { get; set; }

        [Key]
        [Column( Order = 1 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int person_id { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public int role_luid { get; set; }

        public int organization_id { get; set; }

        public virtual Family Family { get; set; }

        public virtual Lookup Role { get; set; }

        public virtual Person Person { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
