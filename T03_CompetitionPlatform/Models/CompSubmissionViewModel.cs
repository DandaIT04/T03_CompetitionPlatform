using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace T03_CompetitionPlatform.Models
{
    public class CompSubmissionViewModel
    {
        [StringLength(255)]
        [Required]
        public string CompetitionName { get; set; }

        [StringLength(50)]
        [Required]
        public string CompetitorName { get; set; }

        [StringLength(5)]
        [RegularExpression(@"[A-Za-z]{1,4}")]
        public string? Salutation { get; set; }

        [Required]
        public int CompetitionID { get; set; }

        [Required]
        public int CompetitorID { get; set; }

        [StringLength(255)]
        public string? FileSubmitted { get; set; }

        [DisplayFormat(DataFormatString = "{yyyy-MM-dd }")]
        public DateTime? DateTimeFileUpload { get; set; }

        [StringLength(255)]
        public string? Appeal { get; set; }

        [Required]
        public int VoteCount { get; set; }

        //[Range(1)]
        public int? Ranking { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }
    }
}
