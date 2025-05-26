using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class SearchArticleDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان مقاله")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string ArticleName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string ShortDescription { get; set; }
 
        [Display(Name = "تصویر ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string ImageAddress { get; set; }


  

    }
}
