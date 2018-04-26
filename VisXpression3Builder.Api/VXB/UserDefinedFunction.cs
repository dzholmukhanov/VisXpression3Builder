using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VisXpression3Builder.Api.VXB
{
    /// <summary>
    /// How to store user-defined function is totally up to a library user
    /// </summary>
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