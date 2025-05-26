using Kids.Hubs;
using Kids.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

[Route("/Admin/Upload")]
public class UploadController : Controller
{
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadController(ILogService logService, IHttpContextAccessor httpContextAccessor)
    {
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }


    [HttpPost("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile upload)
    {

        await _logService.LogActivity("درخواست آپلود عکس برای دوره جدید", _httpContextAccessor.HttpContext);
        if (upload == null || upload.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        // تعیین مسیر ذخیره
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

        // ذخیره فایل در سرور
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await upload.CopyToAsync(stream);
        }

        // بازگشت مسیر فایل برای نمایش در CKEditor
        var url = $"{"/uploads/"}{fileName}";
        return Json(new { uploaded = true, url = url });

    }
}