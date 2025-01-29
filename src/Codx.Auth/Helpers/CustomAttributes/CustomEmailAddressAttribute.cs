using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Codx.Auth.Helpers.CustomAttributes
{
    public class CustomEmailAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Email address is required.");
            }

            var email = value.ToString();
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!regex.IsMatch(email))
            {
                return new ValidationResult("Invalid email address format.");
            }

            return ValidationResult.Success;
        }
    }
}
