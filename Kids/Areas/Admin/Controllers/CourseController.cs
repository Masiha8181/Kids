using Kids.Areas.Admin.AdminDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using DocumentFormat.OpenXml.Office2010.Excel;
using Kids.Services;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
  
    [Admin]
    public class CourseController : Controller
    {
        private readonly KidsContext _context;

        public CourseController(KidsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.Courses.Where(p => p.IsDeleted != true).Include(o => o.Master).Include(p=>p.AgeGroup).Include(i => i.CourseGroup).Include(p=>p.UserCourses).ThenInclude(o=>o.User);

            return View(list);
        }


        public IActionResult Create()
        {
            var Groups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            var Masters = _context.Masters.Where(p => p.IsDeleted != true).ToList();
            var ageList = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Age = ageList;
            ViewBag.Masters = Masters;
            ViewBag.Groups = Groups;
          
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Course.StatusCourse)));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDTO createCourseDto,string? DiscountDate, string? StartDate
        , string? EndDate)
        {
            DateTime? startDate = string.IsNullOrWhiteSpace(StartDate) ? null : DateTime.Parse(StartDate);
            DateTime? endDate = string.IsNullOrWhiteSpace(EndDate) ? null : DateTime.Parse(EndDate);
            DateTime? discountEnd = string.IsNullOrWhiteSpace(DiscountDate) ? null : DateTime.Parse(DiscountDate);

            var Groups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            var Masters = _context.Masters.Where(p => p.IsDeleted != true).ToList();
            var ageList = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Age = ageList;
            ViewBag.Masters = Masters;
            ViewBag.Groups = Groups;
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Course.StatusCourse)));

            if (createCourseDto.ImageAddress == null || createCourseDto.ImageAddress.Length == 0)
            {
                ModelState.AddModelError("ImageAddress", "فایل تصویر انتخاب نشده");
                return View();
            }
            
            if (ModelState.IsValid)
            {
                if (StartDate!=null&&EndDate!=null)
                {
                    if (DateTime.Parse(StartDate) >= DateTime.Parse(EndDate))
                    {
                        ModelState.AddModelError("DateEnd", "تاریخ معتبر نیست");
                        return View();
                    }
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(createCourseDto.ImageAddress.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createCourseDto.ImageAddress.CopyToAsync(stream);
                }

                var imagePath = "/uploads/" + uniqueFileName;

                decimal? totalPrice = null;

                if (createCourseDto.IsFree)
                {
                    totalPrice = 0;
                }
                else if (createCourseDto.BasicPrice != null)
                {
                    totalPrice = createCourseDto.BasicPrice;

                    if (createCourseDto.DiscountStatus && createCourseDto.DiscountPercent != null)
                    {
                        totalPrice = createCourseDto.BasicPrice - (createCourseDto.BasicPrice * createCourseDto.DiscountPercent / 100);
                    }
                }

                Course course = new Course()
                {
                    IsActive = createCourseDto.IsActive,
                    ImageAddress = uniqueFileName,
                    Capacity = createCourseDto.Capacity,
                    CourseGroupId = createCourseDto.CourseGroupId,
                    CourseName = createCourseDto.CourseName,
                    CreateDate = DateTime.Now,
                    DateEnd = endDate,
                    DateStart = startDate,
                    FullDescription = createCourseDto.FullDescription,
                    MasterId = createCourseDto.MasterId,
                    NumberOfSessions = createCourseDto.NumberOfSessions,
                    BasicPrice = createCourseDto.BasicPrice,
                    TotalPrice = totalPrice,
                    IsDeleted = false,
                    AgeGroupId = createCourseDto.AgeGroupId,
                    DiscountEnd = discountEnd,
                    DiscountPercent = createCourseDto.DiscountPercent,
                    DiscountStatus = createCourseDto.DiscountStatus,
                    Tags = createCourseDto.Tags,
                    Slug = SlugMacker.GenerateSlug(createCourseDto.CourseName),
                    ShortDescription = createCourseDto.ShortDescription,
                    Status = createCourseDto.Status,
                    IsFree = createCourseDto.IsFree


                };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();
            }

            return View();
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Courses.FirstOrDefault(p => p.Id == id);
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

            item.IsDeleted = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult Edit(int id)
        {
            var Groups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            var Masters = _context.Masters.Where(p => p.IsDeleted != true).ToList();
            var ageList = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Age = ageList;
            ViewBag.Masters = Masters;
            ViewBag.Groups = Groups;
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Course.StatusCourse)));
            var Course = _context.Courses.Find(id);

            EditCourseDTO courseDto = new EditCourseDTO()
            {
                CourseName = Course.CourseName,
                Capacity = Course.Capacity,
                DateEnd = Course.DateEnd,
                FullDescription = Course.FullDescription,
                ShortDescription = Course.ShortDescription,
                IsActive = Course.IsActive,
                DateStart = Course.DateStart,
                NumberOfSessions = Course.NumberOfSessions,
                BasicPrice = Course.BasicPrice,
                AgeGroupId = Course.AgeGroupId,
                DiscountEnd = Course.DiscountEnd,
                DiscountStatus = Course.DiscountStatus,
                DiscountPercent = Course.DiscountPercent,
                Tags = Course.Tags,
                IsFree = Course.IsFree,
                
                
                Status = Course.Status,
                ImageAddress = Course.ImageAddress,
                Id = Course.Id,
                CourseGroupId = Course.CourseGroupId,
                MasterId = Course.MasterId,

            };
            return View(courseDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditCourseDTO editCourse, IFormFile? ImageUpload, string? DiscountDate, string? StartDate
            , string? EndDate)
        {
            DateTime? startDate = string.IsNullOrWhiteSpace(StartDate) ? null : DateTime.Parse(StartDate);
            DateTime? endDate = string.IsNullOrWhiteSpace(EndDate) ? null : DateTime.Parse(EndDate);
            DateTime? discountEnd = string.IsNullOrWhiteSpace(DiscountDate) ? null : DateTime.Parse(DiscountDate);
            var Groups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            var Masters = _context.Masters.Where(p => p.IsDeleted != true).ToList();
            var ageList = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Age = ageList;
            ViewBag.Masters = Masters;
            ViewBag.Groups = Groups;
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Course.StatusCourse)));
            if (ModelState.IsValid)
            {
                var Course = _context.Courses.Find(editCourse.Id);
                var finalImageAddress = editCourse.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Course.ImageAddress;
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
                if (editCourse.DateStart >= editCourse.DateEnd)
                {
                    ModelState.AddModelError("DateEnd", "تاریخ معتبر نیست");
                    return View();
                }

                decimal? totalPrice = null;

                if (editCourse.IsFree)
                {
                    totalPrice = 0;
                }
                else if (editCourse.BasicPrice != null)
                {
                    totalPrice = editCourse.BasicPrice;

                    if (editCourse.DiscountStatus && editCourse.DiscountPercent != null)
                    {
                        totalPrice = editCourse.BasicPrice - (editCourse.BasicPrice * editCourse.DiscountPercent / 100);
                    }
                }
                Course.ImageAddress = finalImageAddress;
                Course.IsActive = editCourse.IsActive;
                Course.CourseGroupId=editCourse.CourseGroupId;
                Course.CourseName=editCourse.CourseName;
                Course.DateEnd=endDate;
                Course.DateStart = startDate;
                Course.BasicPrice=editCourse.BasicPrice;
                Course.Tags=editCourse.Tags;
                Course.AgeGroupId=editCourse.AgeGroupId;
                Course.DiscountEnd=discountEnd;
                Course.DiscountPercent=editCourse.DiscountPercent;
                Course.IsFree=editCourse.IsFree;
                Course.DiscountStatus=editCourse.DiscountStatus;
                Course.TotalPrice = totalPrice;
                Course.Status=editCourse.Status;
                Course.Capacity=editCourse.Capacity;
                Course.NumberOfSessions=editCourse.NumberOfSessions;
                Course.FullDescription=editCourse.FullDescription;
                Course.ShortDescription=editCourse.ShortDescription;
                Course.MasterId=editCourse.MasterId;
                Course.Slug = editCourse.CourseName;


                _context.SaveChanges();
;                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editCourse);
            }

            return View(editCourse);
        }

        public IActionResult UserList(int Id)
        {
            var UserinCourse = _context.UserCourses.Where(p => p.CourseId == Id).Select(u => u.User).Distinct().ToList();
            var coursename = _context.Courses.Find(Id);
            ViewBag.Name = coursename.CourseName;
            ViewBag.Id = coursename.Id;
            return View(UserinCourse);
        }

        public IActionResult AddUserToCourse(int Id)
        {
            var course=_context.Courses.Find(Id);
            ViewBag.Course = course.CourseName;
            ViewBag.CourseId = course.Id;
            return View();
        }
        [HttpPost]
        public IActionResult AddUserToCourse([FromBody] AddUserToCourseDTO dto)
        {
            if (dto == null || dto.userId == 0  || dto.courseId == 0)
            {
                return BadRequest(new { message = "اطلاعات ناقص است!" });
            }

            // بررسی و اضافه کردن کاربر به دوره
            var user = _context.Users.Find(dto.userId);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد!" });
            }

            var course = _context.Courses.Find(dto.courseId);
            if (course == null)
            {
                return NotFound(new { message = "دوره یافت نشد!" });
            }

            // بررسی عضویت و اضافه کردن کاربر
            var existingUserCourse =
                _context.UserCourses.Where(p => p.CourseId == dto.courseId && p.UserId == dto.userId).FirstOrDefault();

            if (existingUserCourse != null)
            {
                return BadRequest(new { message = "کاربر قبلاً در این دوره عضو شده است!" });
            }

            UserCourse courseMember = new UserCourse()
            {
                CourseId = dto.courseId,
                UserId = dto.userId,

            };
            _context.UserCourses.Add(courseMember);
            _context.SaveChanges();

            return Ok(new { message = "کاربر با موفقیت به دوره اضافه شد!" });
        }
        [HttpPost]
        public IActionResult DeleteUser([FromBody] AddUserToCourseDTO dto)
        {
            var item = _context.UserCourses.FirstOrDefault(p => p.UserId==dto.userId&&p.CourseId==dto.courseId);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

           

            _context.UserCourses.Remove(item);
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult CourseGallery(int id)
        {

            ViewBag.Name = _context.Courses.Find(id).CourseName;
            ViewBag.Id = id;
            var Gallery = _context.CourseGalleries.Where(p => p.CourseId == id).ToList();
            return View(Gallery);
        }

        public IActionResult CreateGallery(int id)
        {
            ViewBag.Name = _context.Courses.Find(id).CourseName;
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateGallery(CreateGalleryDTO createGallery)
        {
            ViewBag.Name = _context.Courses.Find(createGallery.CourseId).CourseName;
            ViewBag.Id = createGallery.CourseId;
            if (createGallery.ImageAddrress == null || createGallery.ImageAddrress.Length == 0)
            {
                ModelState.AddModelError("ImageAddress", "فایل تصویر انتخاب نشده");
                return View();
            }
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Gallery");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createGallery.ImageAddrress.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createGallery.ImageAddrress.CopyToAsync(stream);
                }

                var imagePath = "/uploads/Gallery" + uniqueFileName;
                CourseGallery  courseGallery = new CourseGallery()
                {
                   ImageAddrress = uniqueFileName,
                   ImageTite = createGallery.ImageTitle,
                   CourseId = createGallery.CourseId
                   

                };
                _context.CourseGalleries.Add(courseGallery);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();

            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteGallery(int id)
        {
            var item = _context.CourseGalleries.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Gallery");
            var GalleryImagePath = item.ImageAddrress;
            var filePath = Path.Combine(uploadsFolder, GalleryImagePath);
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

        public IActionResult EditGallery(int id)
        {
         var name = _context.CourseGalleries.Include(p => p.Course).SingleOrDefault(p => p.Id == id);
            ViewBag.Id = _context.CourseGalleries.Find(id).CourseId;
            ViewBag.Name = name;
            var Gallery = _context.CourseGalleries.Find(id);
            EditGalleryDTO editGalleryDto = new EditGalleryDTO()
            {
              ImageAddrress = Gallery.ImageAddrress,
              ImageTitle = Gallery.ImageTite,
              Id = Gallery.Id

            };
            return View(editGalleryDto);
        }
        [HttpPost]
        public async Task<IActionResult> Editgallery(EditGalleryDTO editGalleryDto, IFormFile? ImageUpload)
        {
            var name = _context.CourseGalleries.Include(p => p.Course).SingleOrDefault(p => p.Id == editGalleryDto.Id);
            ViewBag.Id = _context.CourseGalleries.Find(editGalleryDto.Id).CourseId;
            ViewBag.Name = name;
            if (ModelState.IsValid)
            {
                var Gallery = _context.CourseGalleries.Find(editGalleryDto.Id);
                var finalImageAddress = Gallery.ImageAddrress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Gallery");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Gallery.ImageAddrress;
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
                    var imagePath = "/uploads/Gallery" + uniqueFileName;

                }

                Gallery.ImageAddrress = finalImageAddress;
                Gallery.ImageTite = editGalleryDto.ImageTitle;
               
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(editGalleryDto);
            }

            return View(editGalleryDto);
        }
    }


    }

