using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using T03_CompetitionPlatform.DAL;

namespace T03_CompetitionPlatform.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private CompetitorDAL competitorContext = new CompetitorDAL();
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            string email = Convert.ToString(value);
            // Casting the validation context to the "Staff" model class
            Competitor compt = (Competitor)validationContext.ObjectInstance;

            if (competitorContext.IsEmailExist(email))
                // validation failed
                return new ValidationResult
                ("Email address already exists!");
            else
                // validation passed 
                return ValidationResult.Success;
        }
    }
}
