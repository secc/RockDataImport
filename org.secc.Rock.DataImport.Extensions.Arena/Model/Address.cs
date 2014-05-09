using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_address")]
    public partial class Address
    {
        public Address()
        {
            PersonAddress = new HashSet<PersonAddress>();
            Campus = new HashSet<Campus>();
            OrganizationAddress = new HashSet<Organization>();
        }

        [Key]
        public int address_id { get; set; }

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
        public string street_address_1 { get; set; }

        [Required]
        [StringLength(100)]
        public string street_address_2 { get; set; }

        [Required]
        [StringLength(64)]
        public string city { get; set; }

        [Required]
        [StringLength(12)]
        public string state { get; set; }

        [Required]
        [StringLength(24)]
        public string postal_code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int standardize_code { get; set; }

        [Required]
        [StringLength(255)]
        public string standardize_msg { get; set; }

        public int? foreign_key { get; set; }

        public double XAxis { get; set; }

        public double YAxis { get; set; }

        public double ZAxis { get; set; }

        public DateTime date_geocoded { get; set; }

        public DateTime date_standardized { get; set; }

        public int? area_id { get; set; }

        [Required]
        [StringLength(50)]
        public string geocode_service { get; set; }

        public int geocode_status { get; set; }

        [Required]
        [StringLength(50)]
        public string route { get; set; }

        [Required]
        [StringLength(50)]
        public string dpbc { get; set; }

        [Required]
        [StringLength(50)]
        public string lot { get; set; }

        [Required]
        [StringLength(50)]
        public string endorsement_line { get; set; }

        [Required]
        [StringLength(50)]
        public string container_label { get; set; }

        [Required]
        [StringLength(2)]
        public string country { get; set; }

        [Required]
        [StringLength(50)]
        public string bundle_number { get; set; }

        public int organization_id { get; set; }

        [Required]
        [StringLength(31)]
        public string imbc { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<PersonAddress> PersonAddress { get; set; }

        public virtual ICollection<Campus> Campus { get; set; }

        public virtual ICollection<Organization> OrganizationAddress { get; set; }
    }
}
