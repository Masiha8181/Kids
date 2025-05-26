using System.ComponentModel.DataAnnotations;
using Kids.Models;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateGroupsDTO
    {
        
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string GroupName { get; set; }
        public string? Descreption { get; set; }
        public IFormFile? ImageAddress { get; set; }

        public int? ParentID { get; set; }
    }
}
