using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_address")]
    public partial class PersonAddress
    {
        [Key]
        [Column( Order = 0 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int person_id { get; set; }

        [Key]
        [Column( Order = 1 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int address_id { get; set; }

        [Key]
        [Column( Order = 2 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int address_type_luid { get; set; }

        public bool primary_address { get; set; }

        public DateTime? active_date { get; set; }

        public DateTime? inactive_date { get; set; }

        [StringLength( 4 )]
        public string from_month_day { get; set; }

        [StringLength( 4 )]
        public string to_month_day { get; set; }

        [Required]
        [StringLength( 255 )]
        public string notes { get; set; }

        public int organization_id { get; set; }

        public virtual Address Address { get; set; }

        public virtual Lookup AddressType { get; set; }

        public virtual Person Person { get; set; }

        public virtual Organization Organization { get; set; }

    }
}
