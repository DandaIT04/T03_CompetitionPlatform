using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class Judge
    {
        public int JudgeID { get; set; }

        [StringLength(50)]
        [Required]
        public string JudgeName { get; set; }

        [StringLength(5)]
        [RegularExpression(@"[A-Za-z]{1,4}")]
        public string? Salutation { get; set; }

        public int AreaInterestID { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string EmailAddr { get; set; }

        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    } 

    
}
