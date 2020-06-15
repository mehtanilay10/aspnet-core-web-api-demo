using System.ComponentModel.DataAnnotations;
using DemoApp.Models.Attributes;

namespace DemoApp.Models.ApiModels.Auth
{
    public class UserIdDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [ValidateGuid(ErrorMessage = "InvalidMailId")]
        public string UserId { get; set; }
    }
}
