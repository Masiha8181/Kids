using Kids.Models;

namespace Kids.DTO
{
    public class ArticleArchiveDTO
    {
        public List<Article> Articles { get; set; }
        public int PageId { get; set; }
        public int PageCount { get; set; }
        public string Search { get; set; }
    
        public List<int> ArticleGroups { get; set; }
  
    
        public string Order { get; set; }
    }
}
