using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "InvalidMailId")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
