using DocumentFormat.OpenXml.Wordprocessing;
using Kids.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kids.Controllers
{
    
    public class DownloadController : Controller
    {
        private readonly KidsContext _context;

        public DownloadController(KidsContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
                return NotFound();

            if (!episode.IsFree)
            {
                var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
                if (string.IsNullOrEmpty(phoneNumber))
                    return Forbid();

                var user = await _context.Users
                    .Where(u => u.PhoneNumber == phoneNumber && u.IsActive && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return Forbid();

                var hasAccess = await _context.UserCourses
                    .AnyAsync(p => p.UserId == user.Id && p.CourseId == episode.CourseId);

                if (!hasAccess)
                    return Forbid();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", "Episodes", episode.CourseId.ToString());
            var fileName = Path.GetFileName(episode.FilePath); // جلوگیری از مسیر ناامن
            var fullPath = Path.Combine(filePath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var contentType = "application/octet-stream";

            return File(stream, contentType, fileName);
        }

    }
}
