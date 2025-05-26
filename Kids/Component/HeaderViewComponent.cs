using Kids.Context;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Component
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly KidsContext _context;

        public HeaderViewComponent(KidsContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.CourseGroups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.AgeGroups = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.ArticleGroups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();

            return View();
        }
    }
}
