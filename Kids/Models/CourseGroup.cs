using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kids.Models
{
    public class CourseGroup
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string GroupName { get; set; }
        public string? Descreption { get; set; }
        public string? ImageAddress { get; set; }
        public int? ParentId { get; set; }
        
        public bool IsDeleted { get; set; }
        [ForeignKey("ParentId")]
        public  List<CourseGroup> CourseGroups { get; set; }
         public  List<Course> Courses { get; set; }
    }
}
