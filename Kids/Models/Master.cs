using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class Master
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام کامل مدرس")]
       
        [MaxLength(200)]
        public string MasterFullName { get; set; }
        [Display(Name = "تصویر")]
        
        public string? ImageAddress { get; set; }
        [Display(Name = "توضیحات مدرس")]
    
        public string? MasterDescreption { get; set; }
        public bool? IsDeleted { get; set; }

        public   List<Course> Courses { get; set; }
     

    }
}
