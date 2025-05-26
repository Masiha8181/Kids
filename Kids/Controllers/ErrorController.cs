using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kids.Context;
using Kids.Models;

namespace Kids.Controllers
{
    public class ErrorController : Controller
    {
        private readonly KidsContext _context;

        public ErrorController(KidsContext context)
        {
            _context = context;
        }
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            LogError("404 - Not Found");
            return View("404");
        }

        // ✅ خطاهای دیگر
        [Route("Error/Error")]
        public IActionResult GeneralError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exception != null)
            {
                string errorMessage = exception.Error.Message;
                string controllerName = RouteData.Values["controller"]?.ToString() ?? "Unknown";
                string actionName = RouteData.Values["action"]?.ToString() ?? "Unknown";
                string userIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                LogError(errorMessage, controllerName, actionName, userIP);
            }

            return View("Error");
        }

        private void LogError(string message, string controllerName = "Unknown", string actionName = "Unknown", string userIP = "Unknown")
        {
            var errorLog = new ErrorLog
            {
                ErrorMessage = message,
                ControllerName = controllerName,
                ActionName = actionName,
                UserIP = userIP,
                ErrorDate = DateTime.Now
            };

            _context.ErrorLogs.Add(errorLog);
            _context.SaveChanges();
        }
    }
}

