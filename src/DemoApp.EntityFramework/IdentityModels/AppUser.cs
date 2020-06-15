using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DemoApp.EntityFramework.IdentityModels
{
    [Table("AspNetUsers")]
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("First Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Last Name")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "StringMinMaxLength")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Language/Culture")]
        public int Culture { get; set; }

        [DisplayName("Reset Password On Next Login")]
        public bool ResetPasswordOnNextLogin { get; set; }

        #region Security Questions

        [DisplayName("Has Security Questions")]
        public bool HasSecurityQuestions { get; set; }

        [DisplayName("Security Question 1")]
        public int? SecurityQuestion1 { get; set; }

        [DisplayName("Security Answer 1")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "StringMinMaxLength")]
        public string SecurityAnswer1 { get; set; }

        [DisplayName("Security Question 2")]
        public int? SecurityQuestion2 { get; set; }

        [DisplayName("Security Answer 2")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "StringMinMaxLength")]
        public string SecurityAnswer2 { get; set; }

        #endregion

        #region Audit fields

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Joined On")]
        public DateTime? JoinedOn { get; set; }

        [DisplayName("Last Logged On")]
        public DateTime? LastLoggedOn { get; set; }

        #endregion
    }
}
