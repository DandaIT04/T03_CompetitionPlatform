using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class AreaInterest
    {
        public int AreaInterestID { get; set; }

        [StringLength(50)]
        [Required]
        // Custom Validation Attribute for checking area exist
        [ValidateAreaNameExists]
        public string Name { get; set; }

    }
}
