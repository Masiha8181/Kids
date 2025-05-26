using Kids.Models;
using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class ShowUserInformationDTO
    {
       
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
       
        public string Name { get; set; }


        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
  
        public string PhoneNumber { get; set; }

        [Display(Name = " ایمیل")]
        public string? Email { get; set; }
        [Display(Name = " شماره خانه")]
        public int? HomeNumber { get; set; }
        [Display(Name = " جنسیت")]
        public bool? Sex { get; set; }
        [Display(Name = "تاریخ تولد")]

        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ عضویت")]

        public DateTime CreateDate { get; set; }
        public string? ImageAddress { get; set; }

    }
}
