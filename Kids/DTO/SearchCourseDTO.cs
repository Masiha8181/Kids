using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class SearchCourseDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string CourseName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string ShortDescription { get; set; }

    
        [Display(Name = "تصویر ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string ImageAddress { get; set; }
        [Display(Name = "ایا رایگان است؟ ")]
        public bool IsFree { get; set; }
        [Display(Name = "قیمت اولیه ")]


        public decimal? BasicPrice { get; set; }
        [Display(Name = "قیمت نهایی ")]


        public decimal? TotalPrice { get; set; }

        [Display(Name = "درصد تخفیف ")]


        public decimal? DiscountPercent { get; set; }
        [Display(Name = "تاریخ پایان تخفیف ")]


        public DateTime? DiscountEnd { get; set; }



        [Display(Name = "تخفیف فعال است؟ ")]
        public bool DiscountStatus { get; set; }
      




    
    }
}
