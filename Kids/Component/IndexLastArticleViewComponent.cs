using Kids.Context;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Component
{
    public class IndexLastArticleViewComponent:ViewComponent
    {
        private readonly KidsContext _context;

        public IndexLastArticleViewComponent(KidsContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestArticles =
                await Task.Run(() =>
                    _context.Articles.Where(p => p.IsDeleted != true && p.IsActive == true)
                        .OrderByDescending(p => p.CreateDate).Take(4).ToList());


            return View(latestArticles);
        }
    }
}
