using Kids.Models;
using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateEpisodeDTO
    {
   
        [Display(Name = "نام قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string Title { get; set; }
        [Display(Name = "توضیحات قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string Description { get; set; }

        [Display(Name = "فایل قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public IFormFile FilePath { get; set; }

        public int CourseId { get; set; }
    
        public bool IsFree { get; set; }

       
    }
}
