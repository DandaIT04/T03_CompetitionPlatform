using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace T03_CompetitionPlatform.Models
{
    public class Competitor
    {
        [Display(Name = "Competitor ID")]
        public int CompetitorID { get; set; }

        [Display(Name = "Competitor Name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter a name!")]
        public string CompetitorName { get; set; }

        [Display(Name = "Salutation")]
        [StringLength(5)]
        [RegularExpression(@"[A-Za-z]{1,4}")]
        [Required(ErrorMessage = "Please choose a salutation!")]
        public string? Salutation { get; set; }

        [Display(Name = "Email Address")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter an email address!")]
        [ValidateEmailExists]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string EmailAddr { get; set; }

        [Display(Name = "Password")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a password!")]
        public string Password { get; set; }
    }
}
