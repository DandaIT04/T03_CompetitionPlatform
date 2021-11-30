using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using T03_CompetitionPlatform.DAL;

namespace T03_CompetitionPlatform.Models
{
    public class ValidateAreaNameExists : ValidationAttribute
    {
        private AreaInterestDAL areaContext = new AreaInterestDAL();

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            string name = Convert.ToString(value);
            // Casting the validation context to the "Staff" model class
            AreaInterest area = (AreaInterest)validationContext.ObjectInstance;
            // Get the Staff Id from the staff instance
            int areaInterestID = area.AreaInterestID;
            if (areaContext.IsAreaNameExist(name, areaInterestID))
                // validation failed
                return new ValidationResult
                ("Area name already exists!");
            else
                // validation passed
                return ValidationResult.Success;


        }
    }
}
