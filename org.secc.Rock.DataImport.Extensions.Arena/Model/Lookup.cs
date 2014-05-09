using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_lookup")]
    public partial class Lookup
    {
        public Lookup()
        {
            FamilyMemberRole = new HashSet<FamilyMember>();
            AddressType = new HashSet<PersonAddress>();
            PersonInactiveReason = new HashSet<Person>();
            PersonMaritalStatus = new HashSet<Person>();
            PersonMemberStatus = new HashSet<Person>();
            PersonSuffix = new HashSet<Person>();
            PersonTitle = new HashSet<Person>();
            PersonHistoryType = new HashSet<PersonHistory>();
            PersonPhoneType = new HashSet<PersonPhone>();
            PersonPhoneSMSProvider = new HashSet<PersonPhone>();
        }

        public int lookup_id { get; set; }

        public Guid guid { get; set; }

        public int lookup_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_value { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier2 { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier3 { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier4 { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier5 { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier6 { get; set; }

        [Required]
        [StringLength(50)]
        public string lookup_qualifier7 { get; set; }

        [Required]
        [StringLength(2000)]
        public string lookup_qualifier8 { get; set; }

        public int lookup_order { get; set; }

        public bool active { get; set; }

        public bool system_flag { get; set; }

        public int? foreign_key { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int organization_id { get; set; }

        public virtual ICollection<FamilyMember> FamilyMemberRole { get; set; }

       
        public virtual LookupType LookupType { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<PersonAddress> AddressType { get; set; }

        public virtual ICollection<Person> PersonInactiveReason { get; set; }

        public virtual ICollection<Person> PersonMaritalStatus { get; set; }

        public virtual ICollection<Person> PersonMemberStatus { get; set; }

        public virtual ICollection<Person> PersonSuffix { get; set; }

        public virtual ICollection<Person> PersonTitle { get; set; }

        public virtual ICollection<PersonHistory> PersonHistoryType { get; set; }

        public virtual ICollection<PersonPhone> PersonPhoneType { get; set; }

        public virtual ICollection<PersonPhone> PersonPhoneSMSProvider { get; set; }
    }
}
