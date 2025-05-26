using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class EditMasterDTO
    {
        public int Id { get; set; }
        [Display(Name = "نام کامل مدرس")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(200)]
        public string MasterFullName { get; set; }
        [Display(Name = "تصویر")]
        public string? ImageAddress { get; set; }


        public string? MasterDescreption { get; set; }
    }
}
