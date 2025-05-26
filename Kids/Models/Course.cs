using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kids.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Kids.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
       
        public string CourseName { get; set; }
        [Display(Name = "توضیحات کوتاه ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string ShortDescription { get; set; }
        [Display(Name = "توضیحات اصلی ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string FullDescription { get; set; }
        [Display(Name = "تگ ها  ")]
        public string? Tags { get; set; }
        [Display(Name = "تصویر ")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        
        public string ImageAddress { get; set; }
        [Display(Name = "ایا رایگان است؟ ")]
        public bool IsFree { get; set; }
        [Display(Name = "قیمت اولیه ")]
       

        public decimal? BasicPrice { get; set; }
        [Display(Name = "قیمت نهایی ")]
      

        public decimal? TotalPrice { get; set; }

        [Display(Name = "درصد تخفیف ")]
     

        public decimal? DiscountPercent { get; set; }
        [Display(Name = "تاریخ پایان تخفیف ")]


        public DateTime? DiscountEnd { get; set; }

      

        [Display(Name = "تخفیف فعال است؟ ")]
        public bool DiscountStatus { get; set; }
        [Display(Name = "تاریخ شروع ")]
        public DateTime? DateStart { get; set; }
        [Display(Name = "تاریخ  پایان ")]


        public DateTime? DateEnd { get; set; }

        [Display(Name = "تعداد جلسات")]
       

        public int? NumberOfSessions { get; set; }
        public int? MasterId { get; set; }
        [Display(Name = "ظرفیت")]


        public int? Capacity { get; set; }
        [Display(Name = "تاریخ ایجاد ")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "دوره فعال است؟ ")]
        public bool IsActive { get; set; }
        [Display(Name = "وضعیت دوره")]
        public StatusCourse Status { get; set; }
        public enum StatusCourse
        {
          
            NotStarted = 0,
            InProgress = 1,
            Ended = 2,
            Regstering = 3,


        }
        public string? Slug { get; set; }

        public bool IsDeleted { get; set; }
        public int CourseGroupId { get; set; }
        public int? AgeGroupId { get; set; }

        public virtual Master Master { get; set; }
        public  virtual List<Factor> Factors { get; set; }
        public virtual List<UserCourse> UserCourses { get; set; }
      
        public virtual List<CourseGallery> CourseGalleries { get; set; }
        public virtual List<Episode> Episodes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public  virtual  CourseGroup CourseGroup { get; set; }
        public virtual AgeGroup AgeGroup { get; set; }



    }
}
