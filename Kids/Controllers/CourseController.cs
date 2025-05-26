using Kids.Context;
using Kids.DTO;
using Kids.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kids.Controllers
{
    public class CourseController : Controller
    {
        private readonly KidsContext _context;

        public CourseController(KidsContext context)
        {
            _context = context;
        }

        public IActionResult Archive(int PageId = 1, string Search = "", string IsFree = "",
     List<int> courseGroups = null, string Discount = "", List<int> Masters = null, string Order = "",List<int> AgeGroups=null)
        {
            ViewBag.IsFree = IsFree;
            ViewBag.Discount = Discount;
            ViewBag.Masters = _context.Masters.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Groups = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Category = courseGroups;
            ViewBag.Sort = Order;
            ViewBag.AgeGroups = _context.AgeGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.AgeGroup = AgeGroups;
            ViewBag.Search = Search;
            ViewBag.Master = Masters;
            var courses = _context.Courses
                .Include(c => c.CourseGroup)
                .Include(c => c.Master)
                .Include(p=>p.UserCourses)
                .Where(c => c.IsActive && !c.IsDeleted)
                .AsQueryable();

            // فیلتر بر اساس جستجو
            if (!string.IsNullOrEmpty(Search))
            {
                courses = courses.Where(c =>
                    c.CourseName.Contains(Search) || c.ShortDescription.Contains(Search) || c.FullDescription.Contains(Search) || c.Tags.Contains(Search));
            }

            // فقط دوره‌های رایگان
            if (IsFree=="on")
            {
                courses = courses.Where(c => c.IsFree == true||c.BasicPrice==0||c.TotalPrice==0);
            }

         
            if (courseGroups != null && courseGroups.Any())
            {

                foreach (var VARIABLE in courseGroups)
                {
                    courses = courses.Where(c => courseGroups.Contains(c.CourseGroupId));
                }
            }
  // فیلتر بر اساس استادها
            if (Masters != null && Masters.Any())
            {
                foreach (var VARIABLE in Masters)
                {
                    courses = courses.Where(c => Masters.Contains((int)c.MasterId));
                }
            }
            if (AgeGroups != null && AgeGroups.Any())
            {
                foreach (var VARIABLE in AgeGroups)
                {
                    courses = courses.Where(c => AgeGroups.Contains((int)c.AgeGroupId));
                }
            }
            // فیلتر بر اساس تخفیف‌دار بودن
            if (Discount=="on")
            {
                courses = courses.Where(c => c.DiscountPercent > 0&&c.DiscountStatus==true);
            }

            // مرتب‌سازی
            switch (Order)
            {
                case "Date":
                    courses = courses.OrderByDescending(c => c.CreateDate);
                    break;
                case "DateAsc":
                    courses = courses.OrderBy(c => c.CreateDate);
                    break;
                case "High":
                    courses = courses.OrderByDescending(c => c.TotalPrice);
                    break;
                case "Low":
                    courses = courses.OrderBy(c => c.TotalPrice);
                    break;
                case "Best":
                    courses = courses.OrderByDescending(c => c.UserCourses.Count);
                    break;
                default:
                    courses = courses.OrderByDescending(c => c.CreateDate);
                    break;
            }

            // صفحه‌بندی
            int pageSize = 12;
            int skip = (PageId - 1) * pageSize;
            var pagedCourses = courses.Skip(skip).Take(pageSize).ToList();
            
            var model = new CourseArchiveViewModel
            {
                Courses = pagedCourses,
                PageId = PageId,
                PageCount = (int)Math.Ceiling(courses.Count() / (double)pageSize),
                Search = Search,
               
                CourseGroups = courseGroups,
                Masters = Masters,
                Order = Order,
             
                AgeGroups = AgeGroups
            };
            var CountCourse = courses.ToList().Count();
            ViewBag.pageid = PageId;
            ViewBag.maxpage = (int)Math.Ceiling(CountCourse / (double)12);
            return View(model);
        }
        [Route("Course/{id}/{title}")]
        public IActionResult Show(int id, string title)
        {

            if (User.Identity.IsAuthenticated)
            {
                var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
                var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
                var member = _context.UserCourses.Where(p => p.CourseId == id && p.UserId == user.Id);
                if (member.Any())
                {
                    ViewBag.Member = "True";
                }
                if (user.ImageAddress!=null)
                {
                    ViewBag.Image = user.ImageAddress;
                }
            }
            var course=_context.Courses.Where(p=>p.Id==id&&p.IsDeleted!=true&&p.IsActive==true).Include(p=>p.AgeGroup).Include(p=>p.CourseGalleries)
                .Include(p=>p.Master).Include(p=>p.Episodes).Include(p=>p.CourseGroup).Include(p=>p.Comments).ThenInclude(p=>p.User).SingleOrDefault();
            if (course==null)
            {
                return NotFound();
            }

            if (course.Capacity<1)
            {
                TempData["Full"] = "ظرفیت این دوره تکمیل شده است";
            }
            if (course.Status ==Course.StatusCourse.Ended)
            {
                TempData["Ended"] = " این دوره به پایان رسیده و امکان ثبت‌نام وجود ندارد.";
            }
            return View(course);
        }

        [HttpPost]
     

        public IActionResult CommentAdd(int CourseId, string CommentText, int? ParentId)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
            var result = _context.Courses.Where(p => p.Id == CourseId).FirstOrDefault();
            if (result == null || CommentText == null)
            {
                return Json(new { success = false, message = "خظا!" });
            }


            if (ParentId != 0)
            {
                Comment comment = new Comment()
                {
                    IsDeleted = false,
                    IsApproved = false,
                    CommentText = CommentText,
                    CreateTime = DateTime.Now,
                    ParentId = ParentId,
                    CourseId = CourseId,
                    UserId = user.Id
                };
                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            else
            {
                if (ParentId == 0)
                {
                    Comment comment = new Comment()
                    {
                        IsDeleted = false,
                        IsApproved = false,
                        CommentText = CommentText,
                        CreateTime = DateTime.Now,

                        CourseId = CourseId,
                        UserId = user.Id
                    };
                    _context.Comments.Add(comment);
                    _context.SaveChanges();
                }

            }

            return Json(new { success = true });
        }



    }
}
