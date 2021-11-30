using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace T03_CompetitionPlatform.Models
{
    public class CompetitionSubmission
    {
        // This allows Nullable content
        #nullable enable

        [Required]
        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        [Required]
        [Display(Name = "Competitor ID")]
        public int CompetitorID { get; set; }

        [StringLength(255)]
        [Display(Name = "File Submitted")]
        public string? FileSubmitted { get; set; }

        [Display(Name = "DateTime File Upload")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd:0}", NullDisplayText = "No File Upload DateTime")]
        public DateTime? DateTimeFileUpload { get; set; }

        [StringLength(255)]
        [Display(Name = "Appeal")]
        public string? Appeal { get; set; }

        [Display(Name = "Vote Count")]
        public int VoteCount { get; set; }

        //[Range(1)]

        [Display(Name = "Ranking")]
        public int? Ranking { get; set; }
    }
}
