using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class Criteria
    {
        [StringLength(50)]
        [Required]
        public string CriteriaName { get; set; }

        [Required]
        [Range(1, 100)]
        public int Weightage { get; set; }

        public int CriteriaID { get; set; }

        public int CompetitionID { get; set; }
    }
}
