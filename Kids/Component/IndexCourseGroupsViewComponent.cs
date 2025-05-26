using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;

namespace Kids.Component
{
    public class IndexCourseGroupsViewComponent : ViewComponent
    {
        private readonly KidsContext _context;

        public IndexCourseGroupsViewComponent(KidsContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestCourseGroups =
                await Task.Run(() => _context.CourseGroups.Where(p => p.IsDeleted != true).ToList());
             

            return View(latestCourseGroups);
        }
    }
}