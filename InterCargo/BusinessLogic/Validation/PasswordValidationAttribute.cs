using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace InterCargo.BusinessLogic.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            var errors = new List<string>();

            // Check minimum length
            if (password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long.");
            }

            // Check for uppercase letter
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            // Check for lowercase letter
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            // Check for number
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                errors.Add("Password must contain at least one number.");
            }

            // Check for special character
            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                errors.Add("Password must contain at least one special character.");
            }

            if (errors.Any())
            {
                return new ValidationResult(string.Join(" ", errors));
            }

            return ValidationResult.Success;
        }
    }
}