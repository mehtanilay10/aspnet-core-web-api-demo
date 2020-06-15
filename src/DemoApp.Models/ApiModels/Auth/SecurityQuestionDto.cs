using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    public class SecurityQuestionDto
    {
        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Security Question 1")]
        public int SecurityQuestion1 { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("Security Answer 1")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "StringMinMaxLength")]
        public string SecurityAnswer1 { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Security Question 2")]
        public int SecurityQuestion2 { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [DisplayName("Security Answer 2")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "StringMinMaxLength")]
        public string SecurityAnswer2 { get; set; }
    }
}
