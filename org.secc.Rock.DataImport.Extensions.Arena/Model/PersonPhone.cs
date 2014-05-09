using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_phone")]
    public partial class PersonPhone
    {
        [Key]
        [Column( Order = 0 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int person_id { get; set; }

        [Key]
        [Column( Order = 1 )]
        [DatabaseGenerated( DatabaseGeneratedOption.None )]
        public int phone_luid { get; set; }

        [Required]
        [StringLength( 50 )]
        public string phone_number { get; set; }

        [Required]
        [StringLength( 50 )]
        public string phone_ext { get; set; }

        public bool unlisted { get; set; }

        [Required]
        [StringLength( 50 )]
        public string phone_number_stripped { get; set; }

        public bool sms_enabled { get; set; }

        public int? sms_provider_luid { get; set; }

        public int organization_id { get; set; }

        public virtual Lookup PhoneType { get; set; }

        public virtual Lookup SMSProvider { get; set; }

        public virtual Person Person { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
