using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Kids.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام و نام خانوادگی")]
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
        [Display(Name = "عکس پروفایل")]
        public string? ImageAddress { get; set; }
        [Display(Name = " ایمیل")]
        public string? Email { get; set; }
        [Display(Name = " شماره خانه")]
        public int? HomeNumber { get; set; }
        [Display(Name = " جنسیت")]
        public bool? Sex { get; set; }
        [Display(Name = "تاریخ تولد")]

        public DateTime? BirthDate { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(10)]
        public int ActiveCode { get; set; }
        [Display(Name = "تاریخ عضویت")]

        public DateTime CreateDate { get; set; }
        public DateTime LastRequest { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public List<Factor> FactorsList { get; set; }
        public List<UserCourse> ClassMembers { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Rate> Rates { get; set; }
    }
}
