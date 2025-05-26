using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Areas.Admin.Controllers
{
    [Admin]
    [Area("Admin")]
    public class AgeGroupController : Controller
    {
        private readonly KidsContext _context;

        public AgeGroupController(KidsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            return View(list);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAgeGroupDTO createAgeDto)
        {
         
            if (ModelState.IsValid)
            {
                var uniqueFileName = "";
                if (createAgeDto.ImageAddress != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Agegroups");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                     uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createAgeDto.ImageAddress.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createAgeDto.ImageAddress.CopyToAsync(stream);
                    }

                    var imagePath = "/uploads/Agegroups" + uniqueFileName;
                }
                AgeGroup AgeGroup = new AgeGroup()
                {
                    AgeName = createAgeDto.AgeName,
                   ImageAddress = uniqueFileName,
                   IsDeleted = false

                };
                _context.AgeGroups.Add(AgeGroup);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.AgeGroups.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            if (item.ImageAddress != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/AgeGroups");
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
            var item = _context.AgeGroups.Find(id);
            EditAgeGroupDTO  ageGroupDto = new EditAgeGroupDTO()
            {
                Id = item.Id,
                AgeName = item.AgeName,
               ImageAddress = item.ImageAddress

            };
            return View(ageGroupDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditAgeGroupDTO editAgeGroup, IFormFile? ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var item = _context.AgeGroups.Find(editAgeGroup.Id);
                var finalImageAddress = item.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/AgeGroups");
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
                    var imagePath = "/uploads/AgeGroups" + uniqueFileName;

                }

                item.ImageAddress = finalImageAddress;
                item.AgeName = editAgeGroup.AgeName;
           
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editAgeGroup);
            }

            return View(editAgeGroup);
        }
    }
}
