using DocumentFormat.OpenXml.Office2010.Excel;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]

    public class EpisodeController : Controller
    {
        private readonly KidsContext _context;

        public EpisodeController(KidsContext context)
        {
            _context = context;
        }

        public IActionResult ShowCourseEpisode(int id)
        {
            ViewBag.Name = _context.Courses.Find(id).CourseName;
            ViewBag.Id = id;
            var List = _context.Episodes.Where(p => p.CourseId == id);
            return View(List);

        }
        public IActionResult Create(int id)
        {
            ViewBag.Name = _context.Courses.Find(id).CourseName;
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateEpisodeDTO createEpisodeDto)
        {
            ViewBag.Name = _context.Courses.Find(createEpisodeDto.CourseId).CourseName;
            ViewBag.Id = createEpisodeDto.CourseId;
            var isFileNameExist = _context.Episodes.Any(p =>
                p.Title.ToLower() == createEpisodeDto.Title.ToLower() && p.CourseId == createEpisodeDto.CourseId);
            if (isFileNameExist)
            {
                ModelState.AddModelError("Title", "   نام فایل تکراری است نام دیگری انتخاب کنید");
                return View();
            }

            if (createEpisodeDto.FilePath == null || createEpisodeDto.FilePath.Length == 0)
            {
                ModelState.AddModelError("FilePath", "فایل  انتخاب نشده");
                return View();
            }
            if (ModelState.IsValid)
            {
                var uploadsFolder  = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", "Episodes", createEpisodeDto.CourseId.ToString());

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName = createEpisodeDto.Title+Path.GetExtension(createEpisodeDto.FilePath.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createEpisodeDto.FilePath.CopyToAsync(stream);
                }

                var imagePath = "/PrivateFiles/Episodes/" + createEpisodeDto.CourseId;
                Episode episode = new Episode()
                {
                    FilePath = uniqueFileName,
                    Title = createEpisodeDto.Title,
                    CourseId = createEpisodeDto.CourseId,
                    Description = createEpisodeDto.Description,
                    IsFree = createEpisodeDto.IsFree,
                    CreateDate = DateTime.Now,
                    FileSize = createEpisodeDto.FilePath.Length


                };
                _context.Episodes.Add(episode);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteEpisode(int id)
        {
            var item = _context.Episodes.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", "Episodes", item.CourseId.ToString());
            var EpisodePath = item.FilePath;
            var filePath = Path.Combine(uploadsFolder, EpisodePath);
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در حذف فایل: {ex.Message}");

            }

            _context.Remove(item);
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }
        public IActionResult Edit(int id)
        {
            var name = _context.Episodes.Include(p => p.Course).SingleOrDefault(p => p.Id == id);
            ViewBag.Id = name.CourseId;
            ViewBag.Name = name;
            var Episode = _context.Episodes.Find(id);
            EditEpisodeDTO editEpisodeDto = new EditEpisodeDTO()
            {
               FilePath = Episode.FilePath,
               Title = Episode.Title,
               IsFree = Episode.IsFree,
               Description = Episode.Description,
               Id = Episode.Id,

            };
            return View(editEpisodeDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditEpisodeDTO editEpisodeDto, IFormFile? Upload)
        {
         
            var episodefind = _context.Episodes.Find(editEpisodeDto.Id);
            ViewBag.Id = episodefind.CourseId;
            

            if (ModelState.IsValid)
            {
                var Episode = _context.Episodes.Find(editEpisodeDto.Id);
                var finalAddress = Episode.FilePath;
                if (Upload != null)
                {
                    var isFileNameExist = _context.Episodes.Any(p =>
                        p.Title.ToLower() == editEpisodeDto.Title.ToLower() && p.CourseId == episodefind.CourseId&&p.Id!=editEpisodeDto.Id);
                    if (isFileNameExist)
                    {
                        ModelState.AddModelError("Title", "   نام فایل تکراری است نام دیگری انتخاب کنید");
                        return View(editEpisodeDto);
                    }
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", "Episodes", episodefind.CourseId.ToString());
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Episode.FilePath;
                    var pastfilePath = Path.Combine(uploadsFolder, pastFileName);

                    if (System.IO.File.Exists(pastfilePath))
                    {
                        System.IO.File.Delete(pastfilePath);

                    }
                    var uniqueFileName = editEpisodeDto.Title + Path.GetExtension(Upload.FileName);
                    var newfilename = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(newfilename, FileMode.Create))
                    {
                        await Upload.CopyToAsync(stream);
                    }

                    Episode.FileSize = Upload.Length;
                    finalAddress = uniqueFileName;
                   

                }

                Episode.FilePath = finalAddress;
                Episode.Description = editEpisodeDto.Description;
                
                Episode.Title = editEpisodeDto.Title;
                Episode.IsFree = editEpisodeDto.IsFree;
                Episode.CreateDate=DateTime.Now;
                

                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editEpisodeDto);
            }

            return View(editEpisodeDto);
        }

        [HttpPost]
       
       
        public async Task<IActionResult> Download(int Name)
        {
            var episode = await _context.Episodes.FindAsync(Name);
            if (episode == null)
                return NotFound();

           

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", "Episodes", episode.CourseId.ToString());
            var filename = Path.Combine(filePath, episode.FilePath);
            if (!System.IO.File.Exists(filename))
                return NotFound();

            var filetodownload = System.IO.File.ReadAllBytes(filename);
            

        
            var contentType = "application/octet-stream";
            return File(filetodownload, contentType, episode.FilePath);
        }


    }
}
