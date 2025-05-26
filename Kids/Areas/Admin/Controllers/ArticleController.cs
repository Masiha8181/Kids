using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Models;
using Kids.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]
    public class ArticleController : Controller
    {
        private readonly KidsContext _context;

        public ArticleController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.Articles.Where(p => p.IsDeleted != true).Include(o => o.ArticleGroup).ToList();

            return View(list);
        }


        public IActionResult Create()
        {
            var Groups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();


            ViewBag.Groups = Groups;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleDTO createArticleDto)
        {
            var Groups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();


            ViewBag.Groups = Groups;

            if (createArticleDto.ImageAddress == null || createArticleDto.ImageAddress.Length == 0)
            {
                ModelState.AddModelError("ImageAddress", "فایل تصویر انتخاب نشده");
                return View();
            }

            if (ModelState.IsValid)
            {

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Article");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(createArticleDto.ImageAddress.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createArticleDto.ImageAddress.CopyToAsync(stream);
                }

                var imagePath = "/uploads/Article" + uniqueFileName;


                Article article = new Article()
                {
                    IsActive = createArticleDto.IsActive,
                    ImageAddress = uniqueFileName,

                    ArticleGroupId = createArticleDto.ArticleGroupId,
                    ArticleName = createArticleDto.ArticleName,
                    CreateDate = DateTime.Now,
                    Slug = SlugMacker.GenerateSlug(createArticleDto.ArticleName),
                    FullDescription = createArticleDto.FullDescription,



                    IsDeleted = false,

                    Tags = createArticleDto.Tags,

                    ShortDescription = createArticleDto.ShortDescription,



                };
                _context.Articles.Add(article);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();
            }

            return View();
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Articles.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Article");
            var MasterImagePath = item.ImageAddress;
            var filePath = Path.Combine(uploadsFolder, MasterImagePath);
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

            item.IsDeleted = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
            var Groups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();


            ViewBag.Groups = Groups;
            var Article = _context.Articles.Find(id);
            EditArticleDTO articleDto = new EditArticleDTO()
            {
                ArticleName = Article.ArticleName,

                FullDescription = Article.FullDescription,
                ShortDescription = Article.ShortDescription,
                IsActive = Article.IsActive,

                Tags = Article.Tags,




                ImageAddress = Article.ImageAddress,
                Id = Article.Id,
                ArticleGroupId = Article.ArticleGroupId,


            };
            return View(articleDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditArticleDTO editArticleDto, IFormFile? ImageUpload)
        {
            var Groups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();


            ViewBag.Groups = Groups;
            if (ModelState.IsValid)
            {
                var Article = _context.Articles.Find(editArticleDto.Id);
                var finalImageAddress = editArticleDto.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Article");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Article.ImageAddress;
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
                    var imagePath = "/uploads/Article" + uniqueFileName;

                }

                Article.ImageAddress = finalImageAddress;
                Article.IsActive = editArticleDto.IsActive;
                Article.ArticleGroupId = editArticleDto.ArticleGroupId;
                Article.ArticleName = editArticleDto.ArticleName;

                Article.Tags = editArticleDto.Tags;
                Article.Slug = editArticleDto.ArticleName;
                Article.FullDescription = editArticleDto.FullDescription;
                Article.ShortDescription = editArticleDto.ShortDescription;




                _context.SaveChanges();
                ; TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editArticleDto);
            }

            return View(editArticleDto);
        }
    }
}
