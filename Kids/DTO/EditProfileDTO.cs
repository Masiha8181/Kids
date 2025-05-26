using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class EditProfileDTO
    {

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(200)]
        public string Name { get; set; }


        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        [MaxLength(200)]
        [Display(Name = " ایمیل")]
        public string? Email { get; set; }
        [Display(Name = " شماره خانه")]
        public int? HomeNumber { get; set; }
        [Display(Name = " جنسیت")]
        public bool? Sex { get; set; }
        [Display(Name = "تاریخ تولد")]

        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ عضویت")]

     
        public string? ImageAddress { get; set; }
    }
}
