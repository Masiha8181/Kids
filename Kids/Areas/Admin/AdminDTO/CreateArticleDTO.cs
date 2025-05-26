using Kids.Models;
using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateArticleDTO
    {
      

        [Display(Name = "عنوان مقاله")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string ArticleName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string ShortDescription { get; set; }
        [Display(Name = "توضیحات اصلی ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string FullDescription { get; set; }
        [Display(Name = "تگ ها  ")]
        public string? Tags { get; set; }
        [Display(Name = "تصویر ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public IFormFile ImageAddress { get; set; }


        [Display(Name = "تاریخ ایجاد ")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "مقاله فعال است؟ ")]
        public bool IsActive { get; set; }

      
        public int ArticleGroupId { get; set; }


    

    }
}
