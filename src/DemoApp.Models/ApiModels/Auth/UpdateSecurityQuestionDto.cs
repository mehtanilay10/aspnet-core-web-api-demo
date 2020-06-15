using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    public class UpdateSecurityQuestionDto : SecurityQuestionDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "InvalidMailId")]
        [Display(Name = "Email Address")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "StringMinMaxLength")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
