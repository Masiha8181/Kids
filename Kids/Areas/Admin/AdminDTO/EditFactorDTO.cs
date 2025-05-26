using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class EditFactorDTO
    {
        [Key]
        public int Id { get; set; }
        
        public bool IsFinally { get; set; }
    }
}
