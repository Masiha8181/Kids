using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class ChangePasswordDTO
    {

        [Display(Name = "رمز عبور جدید")]
        [MinLength(7, ErrorMessage = "رمز عبور حداقل باید شامل 7 حرف باشد")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string Password { get; set; }
        [Display(Name = "تکرار رمز عبور جدید")]

        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [Compare("Password", ErrorMessage = "تکرار رمز عبور باید با رمز عبور برابر باشد")]

        public string RePassword { get; set; }

    }
}
