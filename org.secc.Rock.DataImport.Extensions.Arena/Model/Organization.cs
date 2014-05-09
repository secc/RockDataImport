using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("orgn_organization")]
    public partial class Organization
    {
        public Organization()
        {
            Address = new HashSet<Address>();
            ArenaAttribute = new HashSet<ArenaAttribute>();
            ArenaAttributeGroup = new HashSet<ArenaAttributeGroup>();
            Family = new HashSet<Family>();
            FamilyMember = new HashSet<FamilyMember>();
            Lookup = new HashSet<Lookup>();
            LookupType = new HashSet<LookupType>();
            Person = new HashSet<Person>();
            PersonAddress = new HashSet<PersonAddress>();
            PersonAttribute = new HashSet<PersonAttribute>();
            PersonAttributeHistory = new HashSet<PersonAttributeHistory>();
            PersonHistory = new HashSet<PersonHistory>();
            PersonPhone = new HashSet<PersonPhone>();
            Campus = new HashSet<Campus>();
        }

        [Key]
        public int organization_id { get; set; }

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
        public string organization_name { get; set; }

        [Required]
        [StringLength(100)]
        public string organization_url { get; set; }

        [Required]
        [StringLength(255)]
        public string organization_desc { get; set; }

        [Required]
        [StringLength(1028)]
        public string organization_notes { get; set; }

        public int? leader_person_id { get; set; }

        public int? address_id { get; set; }

        public int? foreign_key { get; set; }

        public Guid pass_phrase { get; set; }

        public int? blob_id { get; set; }

        [Required]
        [StringLength(30)]
        public string organization_code { get; set; }

        public Guid guid { get; set; }

        public virtual ICollection<Address> Address { get; set; }

        public virtual ICollection<ArenaAttribute> ArenaAttribute { get; set; }

        public virtual ICollection<ArenaAttributeGroup> ArenaAttributeGroup { get; set; }

        public virtual ICollection<Campus> Campus { get; set; }

        public virtual ICollection<Family> Family { get; set; }

        public virtual ICollection<FamilyMember> FamilyMember { get; set; }

        public virtual ICollection<Lookup> Lookup { get; set; }

        public virtual ICollection<LookupType> LookupType { get; set; }

        public virtual Address OrganizationAddress { get; set; }

        public virtual Person OrganizationLeader { get; set; }

        public virtual ICollection<Person> Person { get; set; }

        public virtual ICollection<PersonAddress> PersonAddress { get; set; }

        public virtual ICollection<PersonAttribute> PersonAttribute { get; set; }

        public virtual ICollection<PersonAttributeHistory> PersonAttributeHistory { get; set; }

        public virtual ICollection<PersonHistory> PersonHistory { get; set; }

        public virtual ICollection<PersonPhone> PersonPhone { get; set; }

        
    }
}
