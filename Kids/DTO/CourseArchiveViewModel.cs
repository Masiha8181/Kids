using Kids.Models;

namespace Kids.DTO
{
    public class CourseArchiveViewModel
    {
        public List<Course> Courses { get; set; }
        public int PageId { get; set; }
        public int PageCount { get; set; }
        public string Search { get; set; }
        public bool IsFree { get; set; }
        public List<int> CourseGroups { get; set; }
        public List<int> AgeGroups { get; set; }
        public bool Discount { get; set; }
        public List<int> Masters { get; set; }
        public string Order { get; set; }
    }
}
