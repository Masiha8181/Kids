using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateArticleGroupDTO
    {
        [Display(Name = "نام گروه مقاله")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string GroupName { get; set; }
        public IFormFile? ImageAddress { get; set; }

        public int? ParentID { get; set; }
    }
}
