using System.Diagnostics;
using System.Text.Json;
using Kids.Context;
using Kids.DTO;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kids.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KidsContext _context;

        public HomeController(ILogger<HomeController> logger, KidsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Search(string keyword = "", int coursePage = 1, int articlePage = 1)

        //{
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        int pageSize = 12;

        //        var courseQuery = _context.Courses
        //            .Where(p => p.IsActive==true && p.IsDeleted != true &&
        //                        (p.CourseName.Contains(keyword) || p.FullDescription.Contains(keyword) || p.ShortDescription.Contains(keyword)));

        //        var articleQuery = _context.Articles
        //            .Where(p => p.IsActive==true && p.IsDeleted != true &&
        //                        (p.ArticleName.Contains(keyword) || p.FullDescription.Contains(keyword) || p.ShortDescription.Contains(keyword)));

        //        int skipCourses = (coursePage - 1) * pageSize;
        //        int skipArticles = (articlePage - 1) * pageSize;

        //        var pagedCourses = courseQuery.Skip(skipCourses).Take(pageSize).ToList();
        //        var pagedArticles = articleQuery.Skip(skipArticles).Take(pageSize).ToList();

        //        ViewBag.coursePage = coursePage;
        //        ViewBag.courseMaxPage = (int)Math.Ceiling(courseQuery.Count() / (double)pageSize);

        //        ViewBag.articlePage = articlePage;
        //        ViewBag.articleMaxPage = (int)Math.Ceiling(articleQuery.Count() / (double)pageSize);


        //        FullSearchDTO searchDto = new FullSearchDTO()
        //        {
        //            Courses = pagedCourses,
        //            Articles = pagedArticles
        //        };

        //        return View(searchDto);
        //    }

        //    return View(new FullSearchDTO()); // در صورت خالی بودن keyword
        //}

        [Route("/Search")]
        [HttpGet]
        public IActionResult Search(string KeyWord = "", int CourseLoadID = 0, int ArticleLoadID = 0)
        {
            int LoadSize = 12;

            if (string.IsNullOrWhiteSpace(KeyWord) && CourseLoadID == 0 && ArticleLoadID == 0)
                return View();

            var courseQuery = _context.Courses
                .Where(p => p.IsDeleted != true && p.IsActive == true &&
                            (p.CourseName.Contains(KeyWord) || p.ShortDescription.Contains(KeyWord) || p.FullDescription.Contains(KeyWord)))
                .Include(p => p.CourseGroup);

            var articleQuery = _context.Articles
                .Where(p => p.IsDeleted != true && p.IsActive == true &&
                            (p.ArticleName.Contains(KeyWord) || p.ShortDescription.Contains(KeyWord) || p.FullDescription.Contains(KeyWord)))
                .Include(p => p.ArticleGroup);

            // فقط زمانی که درخواست AJAX است
            if (CourseLoadID > 0)
            {

                var Courses = courseQuery.Skip(CourseLoadID * LoadSize).Take(LoadSize).Select(c => new SearchCourseDTO()
                {
                    TotalPrice = c.TotalPrice,
                    BasicPrice = c.BasicPrice,
                    CourseName = c.CourseName,
                    DiscountPercent = c.DiscountPercent,
                    DiscountStatus = c.DiscountStatus,
                    DiscountEnd = c.DiscountEnd,

                    Id = c.Id,
                    IsFree = c.IsFree,
                    ImageAddress = c.ImageAddress,
                    ShortDescription = c.ShortDescription
                }).ToList();

                return Json(Courses);

            };

            if (ArticleLoadID > 0)
            {

                var Article = articleQuery.Skip(ArticleLoadID * LoadSize).Take(LoadSize).Select(c => new SearchArticleDTO()
                {
                    ArticleName = c.ArticleName,
                    Id = c.Id,
                    ImageAddress = c.ImageAddress,
                    ShortDescription = c.ShortDescription
                }).ToList();

                return Json(Article);

            };


            // بار اولی که صفحه لود می‌شود
            var dto = new FullSearchDTO()
            {
                Courses = courseQuery.Take(LoadSize).ToList(),
                Articles = articleQuery.Take(LoadSize).ToList()
            };

            ViewBag.KeyWord = KeyWord;
            return View(dto);
        }




        public IActionResult Privacy()
        {
            return View();
        }
        [Route("ContactUs")]
        public IActionResult ContactUS()
        {
            return View();
        }
        [Route("ContactUs")]
        [HttpPost]
        public IActionResult ContactUS(ContactUsDTO dto)
        {
            if (ModelState.IsValid)
            {
                ContactUs contactUs = new ContactUs()
                {
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Text = dto.Text
                };
                _context.ContactUs.Add(contactUs);
                _context.SaveChanges();
                TempData["Success"] = "پیام شما دریافت شد";

            }
            return View();
        }

    }
}
