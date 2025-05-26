using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class UserCourse
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public int CourseId { get; set; }

        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
