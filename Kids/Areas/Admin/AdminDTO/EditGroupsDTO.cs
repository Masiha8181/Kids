using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class EditGroupsDTO
    {
        public int Id { get; set; }
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string GroupName { get; set; }
        public string? Descreption { get; set; }
        public string? ImageAddress { get; set; }
        public int? ParentID { get; set; }
    }
}
