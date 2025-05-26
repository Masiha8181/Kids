using Kids.Context;
using Kids.DTO;
using Kids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kids.Controllers
{
    public class ArticleController : Controller
    {
        private readonly KidsContext _context;

        public ArticleController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Archive(int PageId = 1, string Search = "",
  List<int> ArticleGroups = null, string Order = "")
        {


            ViewBag.Groups = _context.ArticleGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.Category = ArticleGroups;
            ViewBag.Sort = Order;


            ViewBag.Search = Search;

            var Article = _context.Articles
                .Include(c => c.ArticleGroup)


                .Where(c => c.IsActive && !c.IsDeleted)
                .AsQueryable();

            // فیلتر بر اساس جستجو
            if (!string.IsNullOrEmpty(Search))
            {
                Article = Article.Where(c =>
                    c.ArticleName.Contains(Search) || c.ShortDescription.Contains(Search) || c.FullDescription.Contains(Search) || c.Tags.Contains(Search));
            }





            if (ArticleGroups != null && ArticleGroups.Any())
            {

                foreach (var VARIABLE in ArticleGroups)
                {
                    Article = Article.Where(c => ArticleGroups.Contains(c.ArticleGroupId));
                }
            }


            // مرتب‌سازی
            switch (Order)
            {
                case "Date":
                    Article = Article.OrderByDescending(c => c.CreateDate);
                    break;
                case "DateAsc":
                    Article = Article.OrderBy(c => c.CreateDate);
                    break;


                case "Best":
                    Article = Article.OrderByDescending(c => c.Rates.Average(P=>P.Value));
                    break;
                default:
                    Article = Article.OrderByDescending(c => c.CreateDate);
                    break;
            }

            // صفحه‌بندی
            int pageSize = 12;
            int skip = (PageId - 1) * pageSize;
            var pagedCourses = Article.Skip(skip).Take(pageSize).ToList();

            var model = new ArticleArchiveDTO
            {
                Articles = pagedCourses,
                PageId = PageId,
                PageCount = (int)Math.Ceiling(Article.Count() / (double)pageSize),
                Search = Search,

                ArticleGroups = ArticleGroups,

                Order = Order,


            };
            var CountCourse = Article.ToList().Count();
            ViewBag.pageid = PageId;
            ViewBag.maxpage = (int)Math.Ceiling(CountCourse / (double)12);
            return View(model);
        }
        [Route("Article/{id}/{slug}")]
        public IActionResult Show(int id, string slug)
        {

            if (User.Identity.IsAuthenticated)
            {
                var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
                var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);

                if (user.ImageAddress != null)
                {
                    ViewBag.Image = user.ImageAddress;
                }

                var oldrate = _context.Rates.SingleOrDefault(p => p.ArticleId == id && p.UserId == user.Id);
                if (oldrate != null)
                {
                    ViewBag.OldRate = oldrate.Value;
                }
            }
            var Article = _context.Articles.Where(p => p.Id == id && p.IsDeleted != true && p.IsActive == true)
     
            .Include(p => p.ArticleGroup).Include(p => p.Comments).ThenInclude(p => p.User).SingleOrDefault();
            ViewBag.Top = _context.Articles.Where(p=>p.IsDeleted!=true&&p.IsActive==true)
                .Include(p => p.Rates)
                .OrderByDescending(p => p.Rates.Any() ? p.Rates.Average(r => r.Value) : 0)
                .Take(3)
                .ToList();

            ViewBag.Last = _context.Articles.Where(p => p.IsDeleted != true && p.IsActive == true).OrderByDescending(p => p.CreateDate).Take(3).ToList();
            if (Article == null)
            {
                return NotFound();
            }

            if (_context.Rates.Any(p => p.ArticleId == id))
            {
                ViewBag.Rate = _context.Rates.Where(p => p.ArticleId == id).Average(p => p.Value);
                ViewBag.RateCount = _context.Rates.Where(p => p.ArticleId == id).Count();
            }
            return View(Article);
        }

        [HttpPost]


        public IActionResult CommentAdd(int ArticleId, string CommentText, int? ParentId)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
            var result = _context.Articles.Where(p => p.Id == ArticleId).FirstOrDefault();
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
                    ArticleId = ArticleId,
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

                        ArticleId = ArticleId,
                        UserId = user.Id
                    };
                    _context.Comments.Add(comment);
                    _context.SaveChanges();
                }

            }

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult AddRate([FromBody] NewRateDTO dto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, messege = "برای دادن امتیاز ابتدا وارد حساب کاربری خود شوید" });
            }

            if (dto.RateValue < 1 || dto.RateValue > 5)
            {
                return Json(new { success = false, messege = "مقدار امتیاز نامعتبر است" });
            }

            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);

            if (user == null)
            {
                return Json(new { success = false, messege = "کاربر یافت نشد یا حساب کاربری فعال نیست" });
            }

            var article = _context.Articles.Find(dto.ArticleId);
            if (article == null)
            {
                return NotFound();
            }

            var oldRate = _context.Rates.SingleOrDefault(p => p.ArticleId == dto.ArticleId && p.UserId == user.Id);
            if (oldRate != null)
            {
                oldRate.Value = dto.RateValue;
                _context.SaveChanges();
            }
            else
            {
                Rate newRate = new Rate()
                {
                    ArticleId = dto.ArticleId,
                    UserId = user.Id,
                    Value = dto.RateValue
                };
                _context.Rates.Add(newRate);
                _context.SaveChanges();
            }

            var average = _context.Rates.Where(p => p.ArticleId == dto.ArticleId).Average(p => p.Value);
            return Json(new { success = true, messege = "امتیاز شما با موفقیت ثبت شد", average = average });
        }

    }
}
