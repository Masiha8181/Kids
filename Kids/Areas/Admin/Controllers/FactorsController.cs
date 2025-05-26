using Kids.Areas.Admin.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Admin]
    public class FactorsController : Controller
    {
        private readonly KidsContext _context;

        public FactorsController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index(string category="All")
        {
            IQueryable<Factor> factors = _context.Factors;
                
            if (category!=null)
            {
                switch (category)
                {
                    case "All":
                        factors = factors;
                        break;
                    case "Active":
                        factors = factors.Where(p => p.IsFinally == true);
                        break;
                    case "Deactive":
                        factors = factors.Where(p => p.IsFinally != true);
                        break;
                }

            }
            var list= factors.Include(p => p.Course)
                .Include(k => k.DiscountCode).Include(m => m.User).ToList();
            
            return View(list);
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Factors.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }


           _context.Factors.Remove(item);

            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int Id)
        {
            var factor = _context.Factors.Find(Id);
            ViewBag.Id = Id;
            EditFactorDTO dto = new EditFactorDTO()
            {
                IsFinally = factor.IsFinally,
                Id = factor.Id
            };
            return View(dto);
        }
        [HttpPost]
        public IActionResult Edit(EditFactorDTO editFactorDto)
        {
            var factor = _context.Factors.Find(editFactorDto.Id);
            ViewBag.Id = editFactorDto.Id;
            if (ModelState.IsValid)
            {
                factor.IsFinally=editFactorDto.IsFinally;
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editFactorDto);
            }
            return View(editFactorDto);
        }
    }
}
