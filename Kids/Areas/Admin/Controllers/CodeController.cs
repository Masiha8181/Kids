using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    [Admin]
    public class CodeController : Controller
    {
        private readonly KidsContext  _context;

        public CodeController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
           var list= _context.DiscountCodes.ToList();
            return View(list);
        }

        public IActionResult Create()
        {
            var master=_context.Masters.Where(p=>p.IsDeleted!=true).ToList();
            ViewBag.List = master;
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateDiscountCodeDTO createDiscountCodeDto)
        {
        
            if (ModelState.IsValid)
            {
                var isexist =
                    _context.DiscountCodes.FirstOrDefault(p =>
                        p.CodeValue == createDiscountCodeDto.CodeValue.ToUpper());
                if (isexist != null)
                {
                    ModelState.AddModelError("CodeValue", "مقدار کد باید یکتا باشد");
                    return View();
                }

                if (createDiscountCodeDto.FixedValue!=null&&createDiscountCodeDto.DiscountPercent!=null)
                {
                    ModelState.AddModelError("CodeValue", "یا مقدار ثابت وارد کنید یا درصد تخفیف");
                    return View();
                }
                if (createDiscountCodeDto.Maximum!=null&&createDiscountCodeDto.Minimum!=null)
                {
                    if (createDiscountCodeDto.Minimum>createDiscountCodeDto.Maximum||createDiscountCodeDto.Maximum<createDiscountCodeDto.Minimum)
                    {
                        ModelState.AddModelError("Maximum","نسبت قیمت حداقل و حداکثر معتبر نیستند");
                        return View();
                    }
                }
                DiscountCode code = new DiscountCode()
                {
                    CodeName = createDiscountCodeDto.CodeName,
                    CodeValue = createDiscountCodeDto.CodeValue.ToUpper(),
                    DiscountPercent = createDiscountCodeDto.DiscountPercent,
                    ExpireDate = createDiscountCodeDto.ExpireDate,
                    IsActive = createDiscountCodeDto.IsActive,
                   
                    MaxUsage = createDiscountCodeDto.MaxUsage,
                    Maximum = createDiscountCodeDto.Maximum,
                    Minimum = createDiscountCodeDto.Minimum,
                    FixedValue = createDiscountCodeDto.FixedValue
                };
                _context.DiscountCodes.Add(code);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ساخته شد";
                return View();
            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.DiscountCodes.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }


            _context.Remove(item);
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
          

            var Code = _context.DiscountCodes.Find(id);
            if (Code != null)
            {
                EditDiscountCodeDTO dto = new EditDiscountCodeDTO()
                {
                    IsActive = (bool)Code.IsActive,
                
                    MaxUsage = Code.MaxUsage,
                    Maximum = Code.Maximum,
                    Minimum = Code.Minimum,
                    CodeValue = Code.CodeValue,
                    CodeName = Code.CodeName,
                    DiscountPercent = Code.DiscountPercent,
                    ExpireDate = Code.ExpireDate,
                    Id = Code.Id,
                    FixedValue = Code.FixedValue,
                };

                return View(dto);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(EditDiscountCodeDTO dto)
        {
           
            var Code = _context.DiscountCodes.Find(dto.Id);
            if (ModelState.IsValid)
            {
                if (Code!=null)
                {
                    var isexist =
                        _context.DiscountCodes.FirstOrDefault(p =>
                            p.CodeValue == dto.CodeValue.ToUpper());
                    if (isexist != null && isexist.Id!=dto.Id)
                    {
                        ModelState.AddModelError("CodeValue", "مقدار کد باید یکتا باشد");
                        return View(dto);
                    }
                    if (dto.FixedValue != null && dto.DiscountPercent != null)
                    {
                        ModelState.AddModelError("CodeValue", "یا مقدار ثابت وارد کنید یا درصد تخفیف");
                        return View();
                    }

                    if (dto.Maximum != null && dto.Minimum != null)
                    {
                        if (dto.Minimum > dto.Maximum || dto.Maximum < dto.Minimum)
                        {
                            ModelState.AddModelError("Maximum", "نسبت قیمت حداقل و حداکثر معتبر نیستند");
                            return View(dto);
                        }
                    }
                   
                    Code.MaxUsage = dto.MaxUsage;
                    Code.Maximum = dto.Maximum;
                    Code.Minimum = dto.Minimum;
                    Code.CodeValue = dto.CodeValue;
                    Code.CodeName = dto.CodeName;
                    Code.DiscountPercent = dto.DiscountPercent;
                    Code.ExpireDate = dto.ExpireDate;
                    Code.IsActive= dto.IsActive;
                    Code.FixedValue=dto.FixedValue;
                    _context.SaveChanges();
                    TempData["Success"]="با موفقیت ویرایش شد";
                    return View(dto);
                }
            }
            return View(dto);
        }

    }
}
