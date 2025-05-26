using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class Episode
    {
        public int Id { get; set; }
        [Display(Name = "نام قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        
        public string Title { get; set; }
        [Display(Name = "توضیحات قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        
        public string Description { get; set; }

        [Display(Name = "فایل قسمت")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int CourseId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsFree { get; set; }

        public virtual Course Course { get; set; }


    }
}
