using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class EditGalleryDTO
    {
        [Key]
        public int Id { get; set; }


        public string? ImageTitle { get; set; }
      

        public string ImageAddrress { get; set; }


       
    }
}
