using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kids.Context;
using System;

namespace Kids.Areas.Admin.Attribute
{
    public class AdminAttribute:ActionFilterAttribute
    {
        private  KidsContext _context;
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            _context =context.HttpContext.RequestServices.GetService<KidsContext>();
            // بررسی اینکه کاربر لاگین کرده یا نه
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }
            var phoneNumber = user.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            // بررسی فیلد IsAdmin (این مقدار باید هنگام ورود در کوکی یا کلایم قرار بگیرد)
            var isAdminClaim = user.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value;
            var userfind = _context.Users.FirstOrDefault(u =>
                u.PhoneNumber ==phoneNumber);

            if (isAdminClaim!="True"||userfind.IsAdmin!=true||userfind==null)
            {
                context.Result = new ForbidResult();  // یا نمایش صفحه عدم دسترسی
            }
        }
    }
}
