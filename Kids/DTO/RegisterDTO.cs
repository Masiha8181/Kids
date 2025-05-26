using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class RegisterDTO
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(100)]
        public string FullName { get; set; }
      

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(12, ErrorMessage = "شماره موبایل معتبر نیست")]
        [Phone(ErrorMessage = "شماره موبایل نا معتبر است")]
        [MinLength(10, ErrorMessage = "شماره موبایل معتبر نیست")]
        public string PhoneNumber { get; set; }


       
        [Display(Name = "رمز عبور")]
        [MinLength(7, ErrorMessage = "رمز عبور حداقل باید شامل 7 حرف باشد")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string Password { get; set; }
     

    }
}
