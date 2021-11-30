using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace T03_CompetitionPlatform.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public int CompetitionID { get; set;}

        [StringLength(255)]
        [Required]
        public string? Description { get; set; }
        public DateTime DateTimePosted { get; set; }
    }
}
