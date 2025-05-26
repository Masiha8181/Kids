using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class LoginWithCodeDTO
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(12, ErrorMessage = "شماره موبایل معتبر نیست")]
        [Phone(ErrorMessage = "شماره موبایل نا معتبر است")]
        [MinLength(10, ErrorMessage = "شماره موبایل معتبر نیست")]
        public string PhoneNumber { get; set; }
    }
}
