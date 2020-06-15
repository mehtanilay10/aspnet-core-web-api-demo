using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoApp.EntityFramework.Entities
{
    public class MstSecurityQuestions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [StringLength(200)]
        public string Question { get; set; }
    }
}
