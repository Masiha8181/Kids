using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class CourseGallery
    {
        [Key]
        public int Id { get; set; }


        public string? ImageTite { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string ImageAddrress { get; set; }

       
        public Course Course { get; set; }
    }
}
