using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateSliderDTO
    {
        [Display(Name = "عنوان اصلی")]


        public string? Title { get; set; }
        [Display(Name = "توضیحات ")]
        public string? Descreption { get; set; }
        [Display(Name = "آدرس لینک")]
        public string? Linkhref { get; set; }
        [Display(Name = "متن لینک")]
        public string? LinkText { get; set; }
        [Display(Name = "تصویر ")]
        public IFormFile ImageAddress { get; set; }
        [Display(Name = " فعال باشد؟")]
        public bool IsActive { get; set; }
    }
}
