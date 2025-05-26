using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class Rate
    {
        [Key]
        public int  Id { get; set; }
        [Required]
        public int Value { get; set; }
        public  int UserId { get; set; }
        public  int ArticleId { get; set; }

        public  virtual User User { get; set; }
        public virtual Article Article { get; set; }

    }
}
