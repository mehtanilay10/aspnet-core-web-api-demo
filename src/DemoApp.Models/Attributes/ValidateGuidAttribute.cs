using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DemoApp.Models.Attributes
{
    /// <summary>
    /// Validation attribute for checking given string is GUID or not
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateGuidAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does not contain a valid guid";

        public ValidateGuidAttribute() : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);

            // let the Required attribute take care of this validation
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            Guid guid;
            if (!Guid.TryParse(input, out guid))
            {
                // not a validstring representation of a GUID
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            // For valid GUID
            return null;
        }
    }
}
