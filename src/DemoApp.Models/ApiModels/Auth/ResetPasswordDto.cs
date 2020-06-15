using System.ComponentModel.DataAnnotations;

namespace DemoApp.Models.ApiModels.Auth
{
    /// <summary>
    /// Model class for reseting password
    /// </summary>
    public class ResetPasswordDto : ResetPasswordOnNextLoginDto
    {
        [Required(ErrorMessage = "RequiredField", AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
