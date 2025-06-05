using Kids.Areas.Admin.AdminDTO;
using Kids.Hubs;
using Kids.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kids.Areas.Admin.Attribute;

[Route("/Admin/Upload")]
[Admin]
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

    [Area("Admin")]
    [HttpGet("ShowUploadFolder")]
    public IActionResult ShowUploadFolder(string? AbsloutePath)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var uploadsRoot = Path.Combine(rootPath, "wwwroot", "uploads");
        var finalPath = uploadsRoot;

        // بررسی امنیتی: جلوگیری از خارج شدن از پوشه uploads
        if (!Directory.Exists(uploadsRoot))
            Directory.CreateDirectory(uploadsRoot);

        if (!string.IsNullOrEmpty(AbsloutePath))
        {
            // نرمال‌سازی مسیر
            AbsloutePath = Path.GetFullPath(AbsloutePath);

            // امنیت: جلوگیری از رفتن بیرون از uploads
            if (AbsloutePath.StartsWith(uploadsRoot))
            {
                finalPath = AbsloutePath;

                var parentDirInfo = Directory.GetParent(AbsloutePath);
                if (parentDirInfo != null && parentDirInfo.FullName.StartsWith(uploadsRoot))
                {
                    ViewBag.ParentFolder = parentDirInfo.FullName;
                }
            }
        }

        // گرفتن لیست دایرکتوری‌ها
        var directoryList = Directory.EnumerateDirectories(finalPath);
        if (directoryList.Any())
        {
            var directoriesWithInfo = directoryList
                .Select(path => new DirectoryInfo(path))
                .ToList();

            ViewBag.Directories = directoriesWithInfo;
        }

        // لیست فایل‌ها
        var fileList = Directory.EnumerateFiles(finalPath);
        var wwwrootPath = Path.Combine(rootPath, "wwwroot");
        var fileListDTO = new List<ShowFileDTO>();

        foreach (var file in fileList)
        {
            var fileInfo = new FileInfo(file);

            var relativePath = fileInfo.FullName
                .Replace(wwwrootPath, "")
                .Replace("\\", "/");

            fileListDTO.Add(new ShowFileDTO
            {
                FilePath = relativePath,
                FileSize = fileInfo.Length.ToString(),
                FileDateModified = fileInfo.CreationTime,
                FileName = fileInfo.Name,
                FileExtension = fileInfo.Extension
            });
        }

        return View(fileListDTO.OrderByDescending(f => f.FileDateModified).ToList());
    }
}