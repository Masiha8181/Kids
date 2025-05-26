using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateMasterDTO
    {
        [Display(Name = "نام کامل مدرس")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(200)]
        public string MasterFullName { get; set; }
        [Display(Name = "تصویر")]

        public IFormFile ImageAddress { get; set; }
        [Display(Name = "توضیحات مدرس")]

        public string? MasterDescreption { get; set; }
    }
}
