using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kids.Models
{
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام کد")]
       

        public string CodeName { get; set; }
        [Display(Name = "کد")]
      

        public string CodeValue { get; set; }

        [Display(Name = "درصد تخفیف کد")]
  

        public decimal? DiscountPercent { get; set; }
        [Display(Name = "مبلغ ثابت  تخفیف ")]


        public decimal? FixedValue { get; set; }
        [Display(Name = "تعداد مجاز استفاده")]
       

        public int? MaxUsage { get; set; }
        [Display(Name = "تعداد  استفاده")]


        public int? UsageCount { get; set; }
        [Display(Name = "حداقل قیمت")]


        public decimal? Minimum { get; set; }
        [Display(Name = "حداکثر قیمت")]


        public decimal? Maximum { get; set; }

        [Display(Name = "تاریخ انقضا")]


        public DateTime? ExpireDate { get; set; }
        [Display(Name = "فعال است؟")]


        public bool? IsActive { get; set; } = false;

       
   
        public   List<Factor> Factors { get; set; }

        public decimal CalculateDiscount(decimal price)
        {
            if (DiscountPercent.HasValue)
                return price - (price * DiscountPercent.Value / 100);
            if (FixedValue.HasValue)
                return price-(decimal)FixedValue;
            return 0;
        }
    }
}
