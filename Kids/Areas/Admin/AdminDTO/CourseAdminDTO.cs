using System.ComponentModel.DataAnnotations;
using Kids.Models;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateCourseDTO
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string CourseName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        public string ShortDescription { get; set; }
        [Display(Name = "توضیحات اصلی ")]
        public string FullDescription { get; set; }
        [Display(Name = "تصویر ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public IFormFile ImageAddress { get; set; }
        [Display(Name = "تگ ها  ")]
        public string? Tags { get; set; }

     
        public bool IsFree { get; set; }
        [Display(Name = "قیمت اولیه ")]
        [Range(0, double.MaxValue, ErrorMessage = "قیمت نمی‌تواند منفی باشد")]

        public decimal? BasicPrice { get; set; }
        [Display(Name = "قیمت نهایی ")]


        public decimal? TotalPrice { get; set; }

        [Display(Name = "درصد تخفیف ")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public decimal? DiscountPercent { get; set; }
        [Display(Name = "تاریخ پایان تخفیف ")]
        

        public DateTime? DiscountEnd { get; set; }



        [Display(Name = "تخفیف فعال است؟ ")]
        public bool DiscountStatus { get; set; }

        [Display(Name = "تاریخ شروع ")]

        public int? AgeGroupId { get; set; }
        public DateTime? DateStart { get; set; }
        [Display(Name = "تاریخ  پایان ")]


        public DateTime? DateEnd { get; set; }

        [Display(Name = "تعداد جلسات")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public int? NumberOfSessions { get; set; }
        public int? MasterId { get; set; }
        [Display(Name = "ظرفیت")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public int? Capacity { get; set; }


        [Display(Name = "دوره فعال است؟ ")]
        public bool IsActive { get; set; }
        [Display(Name = "وضعیت دوره")]
        public Course.StatusCourse Status { get; set; }
      
        
        [Display(Name = "گروه دوره")]
        public int CourseGroupId { get; set; }
    }
    public class EditCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string CourseName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        public string ShortDescription { get; set; }
        [Display(Name = "توضیحات اصلی ")]
        public string FullDescription { get; set; }
        [Display(Name = "تصویر ")]
       

        public string? ImageAddress { get; set; }
        [Display(Name = "تگ ها  ")]
        public string? Tags { get; set; }


        public bool IsFree { get; set; }
        [Display(Name = "قیمت اولیه ")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public decimal? BasicPrice { get; set; }
        [Display(Name = "قیمت نهایی ")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public decimal? TotalPrice { get; set; }

        [Display(Name = "درصد تخفیف ")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public decimal? DiscountPercent { get; set; }
        [Display(Name = "تاریخ پایان تخفیف ")]


        public DateTime? DiscountEnd { get; set; }



        [Display(Name = "تخفیف فعال است؟ ")]
        public bool DiscountStatus { get; set; }

        public int? AgeGroupId { get; set; }


        [Display(Name = "تاریخ شروع ")]


        public DateTime? DateStart { get; set; }
        [Display(Name = "تاریخ  پایان ")]


        public DateTime? DateEnd { get; set; }

        [Display(Name = "تعداد جلسات")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public int? NumberOfSessions { get; set; }
        public int? MasterId { get; set; }
        [Display(Name = "ظرفیت")]

        [Range(0, double.MaxValue, ErrorMessage = " نمی‌تواند منفی باشد")]
        public int? Capacity { get; set; }


        [Display(Name = "دوره فعال است؟ ")]
        public bool IsActive { get; set; }
        [Display(Name = "وضعیت دوره")]
        public Course.StatusCourse Status { get; set; }


        [Display(Name = "گروه دوره")]
        public int CourseGroupId { get; set; }
    }
}
