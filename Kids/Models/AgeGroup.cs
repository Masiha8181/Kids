
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class AgeGroup
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string AgeName { get; set; }

        public string? ImageAddress { get; set; }
        public bool IsDeleted { get; set; }
      
   
        public virtual List<Course> Courses { get; set; }
    }
}
