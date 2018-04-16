using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VisXpression3Builder.Api.VXB
{
    public class UserDefinedFunction
    {
        [Key]
        public string Name { get; set; }
        [Required]
        public string GraphJson { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}