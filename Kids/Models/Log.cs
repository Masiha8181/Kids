using System.ComponentModel.DataAnnotations;

namespace Kids.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string Massege { get; set; }
        public string Address { get; set; }
        public string IP { get; set; }
        public DateTime CreateDate { get; set; }
        public string Device { get; set; }

        public string UserName { get; set; }

    }
}
