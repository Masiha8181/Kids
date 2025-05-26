using System.ComponentModel.DataAnnotations;

namespace Kids.DTO
{
    public class CommentDTO
    {
        public class CreateCommentDTO
        {
            [Display(Name = "نظر ")]


            public string CommentText { get; set; }
            public int? ParentId { get; set; }
            public int CourseId { get; set; }
        }
        public class EditCommentDTO
        {
            public int Id { get; set; }
            [Display(Name = "نظر ")]


            public string CommentText { get; set; }
            
            public bool IsApproved { get; set; }
        }
    }
}
