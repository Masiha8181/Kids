using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class ArticleGroup
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]

        public string GroupName { get; set; }
        public string? ImageAddress { get; set; }
        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; }
        [ForeignKey("ParentId")]
        public List<ArticleGroup> ArticleGroups { get; set; }
        public List<Article>  Articles { get; set; }
    }
}
