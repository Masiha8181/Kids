using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;
using Kids.DTO;

namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Admin]
    public class Users : Controller
    {
        private readonly KidsContext _context;

        public Users(KidsContext context)
        {
            _context = context;
        }

        public IActionResult Edit(int Id)
        {
            var user = _context.Users.Find(Id);
            if (user == null)
            {
                return NotFound();
            }

            EditUserDTO dto = new EditUserDTO()
            {
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Id = user.Id,
               Name = user.Name,
                IsAdmin = user.IsAdmin,
                Password = null,
                RePassword = null
            };

            return View(dto);
        }
        [HttpPost]
        public IActionResult Edit(EditUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                var user=_context.Users.Find(dto.Id);
                var exist = _context.Users.Any(p => p.PhoneNumber == dto.PhoneNumber && p.IsActive == true&&p.Id!=dto.Id);
                if (exist)
                {
                    ModelState.AddModelError("PhoneNumber", "کاربری با این شماره موبایل از قبل موجود است");
                    return View(dto);
                }
                user.PhoneNumber=dto.PhoneNumber;
                user.IsActive = dto.IsActive;
             
                user.Name=dto.Name;
                user.IsAdmin=dto.IsAdmin;
                user.Password=dto.Password;
                user.LastRequest=DateTime.Now;
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                return View(dto);

            }
            return View(dto);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                Random r = new Random();
                var Code = r.Next(1000, 9999);
                var exist = _context.Users.Any(p => p.PhoneNumber == dto.PhoneNumber && p.IsActive == true);
                if (exist)
                {
                    ModelState.AddModelError("PhoneNumber","کاربری با این شماره موبایل از قبل موجود است");
                    return View();
                }

                User user = new User()
                {
                    PhoneNumber = dto.PhoneNumber,
                    IsActive = dto.IsActive,
                    CreateDate = DateTime.Now,
                 
                   Name = dto.Name,
                   IsAdmin = dto.IsAdmin,
                   LastRequest = DateTime.Now,
                   IsDeleted = false,
                   Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                   ActiveCode = Code,
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ایجاد شد";
                return View();
            }
            return View();
        }
        public IActionResult Index( int pageid=1, string Category="All",string order="Asc",string? Search="")
        {
           
         
            ViewBag.Category = Category;
            ViewBag.Order = order;
            ViewBag.Search = Search;
            //var list=_context.Users.Where(o=>o.IsDeleted!=true).Include(p=>p.ClassMembers).ToList();
            IQueryable<User> Query = _context.Users.Where(p=>p.IsDeleted!=true);
            switch (Category)
            {
                case "All":
                    Query = _context.Users;
                    break;

                case "Active":
                    Query = Query.Where(p=>p.IsActive==true&&p.IsDeleted!=true);
                    break;
                case "Deactive":
                    Query = Query.Where(p => p.IsActive == false && p.IsDeleted != true);
                    break;
                case "Buy":
                    Query = Query.Where(p => p.FactorsList.Any(p => p.IsFinally == true));
                    break;
                case "NotBuy":
                    Query = Query.Where(p => !p.FactorsList.Any(p => p.IsFinally == true));
                    break;
            }

            switch (order)
            {
                case "Asc":
                    Query = Query.OrderBy(p => p.Name);
                    break;
                case "Desc":
                    Query = Query.OrderByDescending(p => p.Name);
                    break;
                case "New":
                    Query = Query.OrderBy(p => p.CreateDate);
                    break;
                case "Old":
                    Query = Query.OrderByDescending(p => p.CreateDate);
                    break;
                case "High":
                    Query = Query.OrderByDescending(p => p.FactorsList.Count(f => f.IsFinally == true));
                    break;
            }

            if (!string.IsNullOrEmpty(Search))
            {
                Query = Query.Where(p =>
                    p.Name.ToLower().Contains(Search.ToLower()) ||
                  
                    p.PhoneNumber.Contains(Search)
                );
            }
            var skip = (pageid - 1) * 10;
            var CountUser = Query.ToList().Count();
            Query = Query.Skip(skip).Take(10).Include(c=>c.ClassMembers);
            var list=Query.ToList();
          
            ViewBag.pageid = pageid;
            ViewBag.maxpage = (int)Math.Ceiling(CountUser / (double)10);
                      return View(list);
        }

        public IActionResult UserList(int id)
        {
            var user = _context.Users.Find(id);
            var Course = _context.UserCourses.Where(p => p.UserId == id).Include(s => s.Course.Master)
                .Include(o => o.Course.CourseGroup).Select(c => c.Course).Distinct().ToList();
            ViewBag.User = user.Name;
            ViewBag.Id = user.Id;
            return View(Course);
        }
        public IActionResult GetUserByMobile(string mobile)
        {
            var user = _context.Users.Where(c=>c.PhoneNumber.Contains(mobile)).ToList();



            if (user == null)
                return NotFound();

            return Json(user);
        }
        [HttpPost]
        public IActionResult DeleteCourse([FromBody] AddUserToCourseDTO dto)
        {
            var item = _context.UserCourses.FirstOrDefault(p => p.UserId == dto.userId && p.CourseId == dto.courseId);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }



            _context.UserCourses.Remove(item);
            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult AddUserToCourse(int id)
        {
            var Course = _context.Courses.Where(p => p.IsDeleted != true).ToList();
            var user = _context.Users.Find(id);
            ViewBag.Course=Course;
            ViewBag.Id = id;
            ViewBag.User=user.Name;
            return View();
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Users.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }


            item.IsDeleted = true;

            _context.SaveChanges();

            return Json(new { success = true, message = "حذف شد!" });
        }

        public IActionResult ExportToExcel()
        {
            var users = _context.Users.Select(u => new
            {
                نام = u.Name,
               
                شماره_موبایل = u.PhoneNumber,
                آیا_فعال_است=u.IsActive,
              ـعداد_دوره_ها=u.FactorsList.Where(p=>p.IsFinally==true).Count(),
              جمع_پرداختی=u.FactorsList.Where(p=>p.IsFinally==true).Sum(p=>p.TotalPrice)
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).InsertTable(users);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users"+DateTime.Now+".xlsx");
                }
            }
        }
    }
 
    }

