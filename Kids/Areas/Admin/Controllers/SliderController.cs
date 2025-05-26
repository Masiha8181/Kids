using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]
    
    public class SliderController : Controller
    {
        private readonly KidsContext _context;

        public SliderController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var Slider = _context.Sliders.ToList();
            return View(Slider);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSliderDTO createSliderDto)
        {
            if (createSliderDto.ImageAddress == null || createSliderDto.ImageAddress.Length == 0)
            {
                ModelState.AddModelError("ImageAddress", "فایل تصویر انتخاب نشده");
                return View();
            }
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Slider");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createSliderDto.ImageAddress.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createSliderDto.ImageAddress.CopyToAsync(stream);
                }

                var imagePath = "/uploads/Slider" + uniqueFileName;
                Slider  slider = new Slider()
                {
                    Title = createSliderDto.Title,
                    Descreption = createSliderDto.Descreption,
                    LinkText = createSliderDto.LinkText,
                    Linkhref = createSliderDto.Linkhref,
                    IsActive = createSliderDto.IsActive,
                 
                    ImageAddress = uniqueFileName,
                  

                };
                _context.Sliders.Add(slider);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Sliders.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/slider");
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

            _context.Sliders.Remove(item);
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
            var Slider = _context.Sliders.Find(id);
            EditSliderDTO sliderDto = new EditSliderDTO()
            {
                Title = Slider.Title,
                Descreption = Slider.Descreption,
                Id = Slider.Id,
                ImageAddress = Slider.ImageAddress,
                IsActive = Slider.IsActive,
                LinkText = Slider.LinkText,
                Linkhref = Slider.Linkhref,

            };
            return View(sliderDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditSliderDTO sliderDto, IFormFile? ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var Slider = _context.Sliders.Find(sliderDto.Id);
                var finalImageAddress = Slider.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Slider");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Slider.ImageAddress;
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
                    var imagePath = "/uploads/Slider" + uniqueFileName;

                }

                Slider.ImageAddress = finalImageAddress;
                Slider.Descreption = sliderDto.Descreption;
                Slider.Title = sliderDto.Title;
                Slider.IsActive = sliderDto.IsActive;
                Slider.LinkText=sliderDto.LinkText;
                Slider.Linkhref = sliderDto.Linkhref;
                _context.SaveChangesAsync();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(sliderDto);
            }

            return View(sliderDto);
        }


    }
}
