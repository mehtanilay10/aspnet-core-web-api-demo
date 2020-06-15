using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoApp.EntityFramework
{
    public class AuditFields
    {
        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(20, ErrorMessage = "StringMinMaxLength")]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(20, ErrorMessage = "StringMinMaxLength")]
        [DisplayName("Last Updated By")]
        public string LastUpdatedBy { get; set; }

        [DisplayName("Last Updated On")]
        public DateTime? LastUpdatedOn { get; set; }
    }
}
