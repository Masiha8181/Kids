
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kids.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نظر ")]


        public string CommentText { get; set; }
   

        [Display(Name = "تاریخ ایجاد")]


        public DateTime CreateTime { get; set; }
        [Display(Name = "تایید شود؟")]


        public bool IsApproved { get; set; } = false;

        public int? UserId { get; set; }
        public int? ParentId { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CourseId { get; set; }
        public int? ArticleId { get; set; }
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Article Article { get; set; }

        [ForeignKey("ParentId")]
        public List<Comment> Comments { get; set; }

    }
}
