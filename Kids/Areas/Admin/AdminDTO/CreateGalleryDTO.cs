using Kids.Models;
using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateGalleryDTO
    {




        public string? ImageTitle { get; set; }
        
        public int CourseId { get; set; }
        [Required]
        public IFormFile ImageAddrress { get; set; }


        
    }
}
