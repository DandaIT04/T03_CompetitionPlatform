using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class Competition
    {
        public int CompetitionID { get; set; }

        public int AreaInterestID { get; set; }

        [StringLength(255)]
        [Required]
        [ValidateCompetitionNameExists]
        public string CompetitionName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ResultReleasedDate { get; set; }

        
    }
}
