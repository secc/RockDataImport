using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person")]
    public partial class Person
    {
        public Person()
        {
            FamilyMember = new HashSet<FamilyMember>();
            PersonEmail = new HashSet<PersonEmail>();
            PersonAddress = new HashSet<PersonAddress>();
            PersonAttribute = new HashSet<PersonAttribute>();
            PersonAttributeHistory = new HashSet<PersonAttributeHistory>();
            PersonHistory = new HashSet<PersonHistory>();
            PersonPhone = new HashSet<PersonPhone>();
            CampusLeading = new HashSet<Campus>();
            OrganizationLeading = new HashSet<Organization>();
        }

        [Key]
        public int person_id { get; set; }

        public Guid? guid { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        public DateTime date_last_verified { get; set; }

        [Required]
        [StringLength(50)]
        public string created_by { get; set; }

        [Required]
        [StringLength(50)]
        public string modified_by { get; set; }

        public int? title_luid { get; set; }

        [Required]
        [StringLength(50)]
        public string nick_name { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        public string middle_name { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        public int? suffix_luid { get; set; }

        public DateTime birth_date { get; set; }

        public int gender { get; set; }

        [Required]
        [StringLength(10)]
        public string postal_code { get; set; }

        [Required]
        [StringLength(255)]
        public string Notes { get; set; }

        public int? marital_status { get; set; }

        public DateTime anniversary_date { get; set; }

        public int? member_status { get; set; }

        public int record_status { get; set; }

        [Required]
        [StringLength(1000)]
        public string medical_information { get; set; }

        public int? blob_id { get; set; }

        public int? inactive_reason_luid { get; set; }

        public int? foreign_key { get; set; }

        public int? foreign_key2 { get; set; }

        public int organization_id { get; set; }

        public int active_meter { get; set; }

        public DateTime last_attended { get; set; }

        public bool contribute_individually { get; set; }

        [Required]
        [StringLength(50)]
        public string giving_unit_id { get; set; }

        public DateTime graduation_date { get; set; }

        [MaxLength(500)]
        public byte[] social_security { get; set; }

        public bool print_statement { get; set; }

        public int envelope_number { get; set; }

        public bool? include_on_envelope { get; set; }

        public bool staff_member { get; set; }

        public int? campus_id { get; set; }

        public bool business { get; set; }

        [Required]
        [StringLength(25)]
        public string first_name_metaphone { get; set; }

        [Required]
        [StringLength(25)]
        public string nick_name_metaphone { get; set; }

        [Required]
        [StringLength(25)]
        public string last_name_metaphone { get; set; }

        public bool? restricted { get; set; }

        public bool email_statement { get; set; }

        [MaxLength(500)]
        public byte[] pin { get; set; }

        public virtual ICollection<FamilyMember> FamilyMember { get; set; }

        public virtual Lookup InactiveReason { get; set; }

        public virtual Lookup MaritalStatus { get; set; }

        public virtual Lookup MemberStatus { get; set; }

        public virtual Lookup NameSuffix { get; set; }

        public virtual Lookup NameTitle { get; set; }

        public virtual ICollection<PersonEmail> PersonEmail { get; set; }

        public virtual ICollection<PersonAddress> PersonAddress { get; set; }

        public virtual ICollection<PersonAttribute> PersonAttribute { get; set; }

        public virtual ICollection<PersonAttributeHistory> PersonAttributeHistory { get; set; }

        public virtual ICollection<PersonHistory> PersonHistory { get; set; }

        public virtual Campus Campus { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<PersonPhone> PersonPhone { get; set; }

        public virtual ICollection<Campus> CampusLeading { get; set; }

        public virtual ICollection<Organization> OrganizationLeading { get; set; }

        public virtual Blob Blob { get; set; }
    }
}
