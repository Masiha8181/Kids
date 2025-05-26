using Microsoft.AspNetCore.Mvc;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using System.IO;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]
    public class MastersController : Controller
    {
        private readonly KidsContext _context;

        public MastersController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var masters = _context.Masters.Where(p=>p.IsDeleted!=true).ToList();
            return View(masters);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMasterDTO createMasterDto)
        {
            if (createMasterDto.ImageAddress == null || createMasterDto.ImageAddress.Length == 0)
            {
                ModelState.AddModelError("ImageAddress","فایل تصویر انتخاب نشده");
                return View();
            }
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
            

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createMasterDto.ImageAddress.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                   await createMasterDto.ImageAddress.CopyToAsync(stream);
                }

                var imagePath = "/uploads/" + uniqueFileName;
                Master master = new Master()
                {
                    MasterFullName = createMasterDto.MasterFullName,
                    MasterDescreption = createMasterDto.MasterDescreption,
                    ImageAddress = uniqueFileName,
                    IsDeleted = false
                     
                };
                _context.Masters.Add(master);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Masters.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
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

            item.IsDeleted=true;
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
            var master = _context.Masters.Find(id);
            EditMasterDTO masterDTO = new EditMasterDTO()
            {
                MasterFullName = master.MasterFullName,
                MasterDescreption = master.MasterDescreption,
                Id = master.Id,
                ImageAddress = master.ImageAddress
               
            };
            return View(masterDTO);
        }
        [HttpPost]
        public async  Task<IActionResult> Edit(EditMasterDTO editMasterDto,IFormFile? ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var master = _context.Masters.Find(editMasterDto.Id);
                var finalImageAddress = master.ImageAddress;
                if (ImageUpload != null )
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = master.ImageAddress;
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
                    var imagePath = "/uploads/" + uniqueFileName;

                }
               
                master.ImageAddress = finalImageAddress;
                master.MasterDescreption = editMasterDto.MasterDescreption;
                master.MasterFullName = editMasterDto.MasterFullName;
                _context.SaveChangesAsync();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editMasterDto);
            }

            return View(editMasterDto);
        }

    }
}
