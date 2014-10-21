using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("orgn_campus")]
    public partial class Campus
    {
        public Campus()
        {
            Person = new HashSet<Person>();
        }

        [Key]
        public int campus_id { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_modified { get; set; }

        [Required]
        [StringLength( 50 )]
        public string created_by { get; set; }

        [Required]
        [StringLength( 50 )]
        public string modified_by { get; set; }

        public Guid guid { get; set; }

        public int organization_id { get; set; }

        [Required]
        [StringLength( 100 )]
        public string name { get; set; }

        [Required]
        [StringLength( 100 )]
        public string url { get; set; }

        [Required]
        public string description { get; set; }

        public int? leader_person_id { get; set; }

        public int? address_id { get; set; }

        public int? foreign_key { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<Person> Person { get; set; }

        public virtual Person CampusLeader { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
