using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]
    public class ArticleGroupController : Controller
    {
        private readonly KidsContext _context;

        public ArticleGroupController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            return View(list);
        }
        public IActionResult Create()
        {
            var list = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleGroupDTO createArticleGroupDto)
        {
            var list = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            if (ModelState.IsValid)
            {
                var uniqueFileName = "";
                if (createArticleGroupDto.ImageAddress != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArticleGroups");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createArticleGroupDto.ImageAddress.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createArticleGroupDto.ImageAddress.CopyToAsync(stream);
                    }

                    var imagePath = "/uploads/ArticleGroups" + uniqueFileName;
                }
                ArticleGroup articleGroup = new ArticleGroup()
                {
                    GroupName = createArticleGroupDto.GroupName,
                    ImageAddress = uniqueFileName,
                    IsDeleted = false,
                    ParentId = createArticleGroupDto.ParentID

                };
                _context.ArticleGroups.Add(articleGroup);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.ArticleGroups.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            if (item.ImageAddress != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArticleGroups");
                var ImagePath = item.ImageAddress;
                var filePath = Path.Combine(uploadsFolder, ImagePath);
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
            }

            item.IsDeleted = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
            var list = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            var item = _context.ArticleGroups.Find(id);
            EditArticleGroupDTO editAgeGroup = new EditArticleGroupDTO()
            {
                Id = item.Id,
                GroupName = item.GroupName,
                ImageAddress = item.ImageAddress,
                ParentID = item.ParentId

            };
            return View(editAgeGroup);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditArticleGroupDTO editArticleGroup, IFormFile? ImageUpload)
        {
            var list = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            if (ModelState.IsValid)
            {
                var item = _context.ArticleGroups.Find(editArticleGroup.Id);
                var finalImageAddress = item.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ArticleGroups");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = item.ImageAddress;
                    var pastfilePath = Path.Combine(uploadsFolder, pastFileName);

                    if (System.IO.File.Exists(pastfilePath))
                    {
                        System.IO.File.Delete(pastfilePath);

                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                    var newfilename = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(newfilename, FileMode.Create))
                    {
                        await ImageUpload.CopyToAsync(stream);
                    }

                    finalImageAddress = uniqueFileName;
                    var imagePath = "/uploads/ArticleGroups" + uniqueFileName;

                }

                item.ImageAddress = finalImageAddress;
                item.GroupName = editArticleGroup.GroupName;
                item.ParentId = editArticleGroup.ParentID;

                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editArticleGroup);
            }

            return View(editArticleGroup);
        }
    }
}

