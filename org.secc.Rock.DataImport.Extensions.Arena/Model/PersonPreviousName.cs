using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace org.secc.Rock.DataImport.Extensions.Arena.Model
{
    [Table("core_person_previous_name")]
    public partial class PersonPreviousName
    {
        public PersonPreviousName()
        { }

        [Key]
        public Guid guid { get; set; }

        public int person_id { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        public virtual Person Person { get; set; }
    }
}
