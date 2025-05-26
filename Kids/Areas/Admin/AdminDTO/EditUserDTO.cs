using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class EditUserDTO
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(100)]
        public string Name { get; set; }


        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(100)]
        public string PhoneNumber { get; set; }

     
        [Display(Name = "رمز عبور")]
        [MinLength(7, ErrorMessage = "رمز عبور حداقل باید شامل 7 حرف باشد")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string Password { get; set; }
        [Display(Name = " تکرار رمز عبور")]

        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [Compare("Password", ErrorMessage = "تکرار رمز عبور باید با رمز عبور برابر باشد")]

        public string RePassword { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}
