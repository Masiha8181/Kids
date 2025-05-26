using System.ComponentModel.DataAnnotations;
namespace Kids.Models
{
    public class Factor
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int UserId { get; set; }
        [Required]
        public int CourseId { get; set; }
        public int? DiscountCodeId { get; set; }
        public decimal CourseTotalPrice { get; set; }
        public DateTime FactorDate { get; set; }
        [Required]

        public decimal TotalPrice { get; set; }
        public bool IsFinally { get; set; }
        public string? RefID { get; set; }
        public   User User { get; set; }
        public  Course Course { get; set; }
        public  DiscountCode DiscountCode { get; set; }
        public void ApplyDiscount(DiscountCode discountCode)
        {
            var discount = discountCode.CalculateDiscount(CourseTotalPrice);
         
            TotalPrice = CourseTotalPrice - Math.Min(discount, CourseTotalPrice);
        }
    }
}
