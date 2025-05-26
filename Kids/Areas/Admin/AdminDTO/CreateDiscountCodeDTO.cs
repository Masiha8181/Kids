using Kids.Models;
using System.ComponentModel.DataAnnotations;

namespace Kids.Areas.Admin.AdminDTO
{
    public class CreateDiscountCodeDTO
    {
      
        [Display(Name = "نام کد")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]


        public string CodeName { get; set; }
        [Display(Name = "کد")]
        [Required(ErrorMessage = "فیلد {0} الزامی است")]


        public string CodeValue { get; set; }

        [Display(Name = "درصد تخفیف کد")]
     
        [Range(0, 100, ErrorMessage = "مقدار {0} باید بین {1} تا {2} باشد.")]

        public decimal? DiscountPercent { get; set; }
        [Display(Name = "مبلغ ثابت  تخفیف ")]


        public decimal? FixedValue { get; set; }
        [Display(Name = "تعداد مجاز استفاده")]
       


        public int? MaxUsage { get; set; }
        [Display(Name = "حداقل قیمت")]


        public decimal? Minimum { get; set; }
        [Display(Name = "حداکثر قیمت")]


        public decimal? Maximum { get; set; }

        [Display(Name = "تاریخ انقضا")]


        public DateTime? ExpireDate { get; set; }
        [Display(Name = "فعال است؟")]


        public bool IsActive { get; set; }

        
        
    }

}
