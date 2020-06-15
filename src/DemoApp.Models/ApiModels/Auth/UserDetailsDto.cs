using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DemoApp.Models.Attributes;
using DemoApp.Models.Enums;

namespace DemoApp.Models.ApiModels.Auth
{
    public class UserDetailsDto
    {
        #region Fields

        #region Basic user details

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [ValidateGuid]
        public string UserId { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("User Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "InvalidMailId")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "StringMinMaxLength")]
        public string Email { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [StringLength(20)]
        [Phone(ErrorMessage = "InvalidPhone")]
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("First Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("Last Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string LastName { get; set; }

        [DisplayName("Language/Culture")]
        public SupportedCulture Culture { get; set; }

        #endregion

        #region Additional Fields

        [DisplayName("Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [DisplayName("Reset Password On Next Login")]
        public bool ResetPasswordOnNextLogin { get; set; }

        [DisplayName("Has Security Questions")]
        public bool HasSecurityQuestions { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("Role")]
        public string Role { get; set; }

        #endregion

        #endregion
    }
}
