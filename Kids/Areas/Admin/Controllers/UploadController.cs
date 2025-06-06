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

        string currentPathRelative = ""; 

        if (!string.IsNullOrEmpty(AbsloutePath))
        {
            
            currentPathRelative = AbsloutePath.TrimStart('\\', '/');
        }

        
        string serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", currentPathRelative);

        
        if (!Directory.Exists(serverPath))
        {
            Directory.CreateDirectory(serverPath);
        }

        ViewBag.CurrentPath = currentPathRelative; 


        return View(fileListDTO.OrderByDescending(f => f.FileDateModified).ToList());
    }

    [HttpPost("UploadFile")]
    public async Task<IActionResult> UploadFile(string Path, IFormFile formFile)
    {
        if (formFile != null && formFile.Length > 0)
        {
            try
            {
                var root = Directory.GetCurrentDirectory();
                var safePath = Path ?? ""; 

            
                safePath = safePath.Replace("..", "").TrimStart('/', '\\');

                var uploadPath = System.IO.Path.Combine(root, "wwwroot", "uploads", safePath);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileExtension = System.IO.Path.GetExtension(formFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;

                var fileLocation = System.IO.Path.Combine(uploadPath, fileName);
                using (var stream = System.IO.File.Create(fileLocation))
                {
                    await formFile.CopyToAsync(stream);
                }

                return Json(new { success = true, fileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در آپلود فایل", error = ex.Message });
            }
        }

        return Json(new { success = false, message = "فایلی انتخاب نشده یا فایل خالی است." });
    }
    [HttpPost("CreateDirectory")]
    public IActionResult CreateDirectory(string path, string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName) || folderName.Any(Path.GetInvalidFileNameChars().Contains))
        {
            return Json(new { success = false, message = "نام پوشه نامعتبر است" });
        }

        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        var targetPath = Path.Combine(rootPath, path ?? "", folderName);

     
      
        if (Directory.Exists(targetPath))
        {
            return Json(new { success = false, message = "این پوشه از قبل وجود دارد" });
        }

        Directory.CreateDirectory(targetPath);

        return Json(new { success = true, message = "پوشه با موفقیت ساخته شد" });
    }
    [HttpPost("FolderRemove")]
    public IActionResult FolderRemove(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return Json(new { success = false, message = "مسیر فولدر مشخص نشده است." });

            // مسیر کامل فولدر رو بساز
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", path);

            if (!Directory.Exists(fullPath))
                return Json(new { success = false, message = "فولدر مورد نظر وجود ندارد." });

            // حذف فولدر به همراه تمام محتویاتش
            Directory.Delete(fullPath, true);

            return Json(new { success = true, message = "فولدر با موفقیت حذف شد." });
        }
        catch (Exception ex)
        {
            // می‌تونی لاگ هم بزنی اینجا
            return Json(new { success = false, message = "خطا در حذف فولدر: " + ex.Message });
        }
    }
    [HttpPost("FileRemove")]
    public IActionResult FileRemove(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return Json(new { success = false, message = "مسیر فایل مشخص نشده است." });

            // جلوگیری از دسترسی به مسیرهای بالاتر (مانند ../)
            if (path.Contains(".."))
                return Json(new { success = false, message = "مسیر نامعتبر است." });

            // حذف اسلش اولیه اگر بود
            path = path.TrimStart('/', '\\');

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", path);

            if (!System.IO.File.Exists(fullPath))
                return Json(new { success = false, message = "فایل مورد نظر وجود ندارد." });

            System.IO.File.Delete(fullPath);

            return Json(new { success = true, message = "فایل با موفقیت حذف شد." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "خطا در حذف فایل: " + ex.Message });
        }
    }



}
