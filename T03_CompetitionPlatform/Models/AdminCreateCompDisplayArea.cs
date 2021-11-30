using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

// 7/7/2021

namespace T03_CompetitionPlatform.Models
{
    public class AdminCreateCompDisplayArea
    {
        public List<AreaInterest> areaList { get; set; }

        public List<Competition> compList { get; set; }

        public int CompetitionID { get; set; }

        public int AreaInterestID { get; set; }

        [StringLength(255)]
        [Required]
        [ValidateCompetitionNameExists]
        public string CompetitionName { get; set; }

        // CHANGE IF NECCESSARY IN THE FUTURE
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ResultReleasedDate { get; set; }

        public AdminCreateCompDisplayArea()
        {
            areaList = new List<AreaInterest>();
            compList = new List<Competition>();
        }
    }
}
