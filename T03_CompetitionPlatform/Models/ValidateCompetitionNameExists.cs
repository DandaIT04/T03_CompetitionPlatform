using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using T03_CompetitionPlatform.DAL;

namespace T03_CompetitionPlatform.Models
{
    public class ValidateCompetitionNameExists : ValidationAttribute
    {
        private CompetitionDAL competitionContext = new CompetitionDAL();

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            string competitionName = Convert.ToString(value);
            // Casting the validation context to the "Staff" model class
            Competition competition = (Competition)validationContext.ObjectInstance;
            // Get the Staff Id from the staff instance
            int competitionID = competition.CompetitionID;
            if (competitionContext.IsCompetitionNameExist(competitionName, competitionID))
                // validation failed
                return new ValidationResult
                ("Competition name already exists!");
            else
                // validation passed
                return ValidationResult.Success;


        }
    }
}
