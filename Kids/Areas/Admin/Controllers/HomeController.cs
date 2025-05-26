using Microsoft.AspNetCore.Mvc;
using Kids.Context;
using Kids.Areas.Admin.Attribute;
using Kids.DTO;

namespace NFF.Areas.Admin.Controllers
{
    
    [Admin]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly  KidsContext _context;

        public HomeController(KidsContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

    
    }
}
