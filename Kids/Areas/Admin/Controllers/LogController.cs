using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Areas.Admin.Controllers
{
    [Admin]
    [Area("Admin")]
    public class LogController : Controller
    {
        private readonly KidsContext _context;

        public LogController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.Logs.Take(100).ToList();
            return View(list);
        }
    }
}
