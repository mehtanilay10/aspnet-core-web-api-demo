using System;
using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.Attributes
{
    /// <summary>
    /// Validate Phone number with RegEx
    /// Note: It allows spaces, hypen, and braces between number
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidatePhoneNumberAttribute : RegularExpressionAttribute
    {
        private const string _phoneNumberPattern = @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$";

        public ValidatePhoneNumberAttribute() : base(_phoneNumberPattern)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} is not a valid Phone number.";
        }
    }
}
