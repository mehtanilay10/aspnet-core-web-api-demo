using System.ComponentModel.DataAnnotations;
using DemoApp.Models.Attributes;

namespace DemoApp.Models.ApiModels.Auth
{
    /// <summary>
    /// Model class for confirming user's mail id
    /// </summary>
    public class ValidateConfirmUserDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        [ValidateGuid]
        public string UserId { get; set; }

        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
