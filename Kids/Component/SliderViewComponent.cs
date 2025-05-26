using Kids.Context;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Component
{
    public class SliderViewComponent:ViewComponent
    {
        private readonly KidsContext _context;

        public SliderViewComponent(KidsContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var list = _context.Sliders.Where(p=>p.IsActive==true).ToList();

            return View(list);
        }
    }
}
