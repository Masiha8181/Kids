using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = " نام و نام خانوادگی")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(100)]
        public string FullName { get; set; }
      
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        [MaxLength(100)]
        public string PhoneNumber { get; set; }

        [Display(Name = "متن پیام ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
     
        public string Text { get; set; }


    }
}
