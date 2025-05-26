using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kids.Context;
using Kids.Areas.Admin.Attribute;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    [Admin]
    public class MessegeController : Controller
    {
        private readonly  KidsContext _context;

        public MessegeController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list=_context.ContactUs.ToList();
            return View(list);
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.ContactUs.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }


            _context.ContactUs.Remove(item);

            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }
    }
}
