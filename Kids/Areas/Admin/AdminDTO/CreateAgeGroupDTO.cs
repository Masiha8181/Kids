using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateAgeGroupDTO
    {
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string AgeName { get; set; }

        public IFormFile? ImageAddress { get; set; }
    }
}
