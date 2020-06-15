using System;
using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.Attributes
{
    /// <summary>
    /// Validate Email address with RegEx
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateEmailAttribute : RegularExpressionAttribute
    {
        private const string _emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
            + "@"
            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        public ValidateEmailAttribute() : base(_emailPattern)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} is not a valid e-mail address.";
        }
    }
}
