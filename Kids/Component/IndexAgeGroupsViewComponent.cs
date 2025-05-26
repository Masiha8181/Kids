using Kids.Context;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Component
{
    public class IndexAgeGroupsViewComponent:ViewComponent
    {
        private readonly KidsContext _context;

        public IndexAgeGroupsViewComponent(KidsContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestCourseGroups =
                await Task.Run(() => _context.AgeGroups.Where(p => p.IsDeleted != true).ToList());


            return View(latestCourseGroups);
        }
    }
}
