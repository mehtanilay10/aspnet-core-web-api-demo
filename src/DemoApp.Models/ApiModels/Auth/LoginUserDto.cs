using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    /// <summary>
    /// Model that used for sending Login details to API
    /// </summary>
    public class LoginUserDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "InvalidMailId")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
