using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("First Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("Last Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "InvalidMailId")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "StringMinMaxLength")]
        public string Email { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [StringLength(20, ErrorMessage = "StringMinMaxLength")]
        [Phone(ErrorMessage = "InvalidPhone")]
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
