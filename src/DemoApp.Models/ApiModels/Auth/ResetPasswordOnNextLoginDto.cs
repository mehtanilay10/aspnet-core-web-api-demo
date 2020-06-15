using System.ComponentModel.DataAnnotations;
using DemoApp.Models.Attributes;

namespace DemoApp.Models.ApiModels.Auth
{
    public class ResetPasswordOnNextLoginDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [ValidateGuid]
        public string UserId { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "StringMinMaxLength")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
