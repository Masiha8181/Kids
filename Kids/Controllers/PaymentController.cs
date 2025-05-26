using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Kids.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly KidsContext _context;
        private readonly HttpClient _httpClient;

        public PaymentController(KidsContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public IActionResult CreateFactor(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["NotLogin"] = "True";
                return RedirectToAction("Show", "Course", new { id = id });
            }
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();
            var course = _context.Courses.Find(id);
            if (!course.IsActive || course.IsDeleted)
            {
                return NotFound();
            }
            updateCoursePrice(id);
            if (course.Status == Course.StatusCourse.Ended)
            {
                TempData["Ended"] = "True";
                return RedirectToAction("Show", "Course", new { id = course.Id });
            }
            if (course.Capacity <1)
            {
                TempData["Full"] = "True";
                return RedirectToAction("Show", "Course", new { id = course.Id });
            }

            var UserCourselist = _context.UserCourses.Where(p => p.CourseId == course.Id && p.UserId == user.Id)
                .SingleOrDefault();
            if (UserCourselist != null)
            {
                TempData["ALready"] = "True";
                return RedirectToAction("Show", "Course", new { id = course.Id });
            }

            var FinalPrice = course.TotalPrice;
            if (course.IsFree == true || course.TotalPrice <= 0 || course.BasicPrice <= 0)
            {
                Factor factor = new Factor()
                {
                    CourseId = course.Id,
                    CourseTotalPrice = 0,
                    FactorDate = DateTime.Now,
                    IsFinally = true,
                    TotalPrice = 0,
                    UserId = user.Id,
                };
                _context.Factors.Add(factor);
                UserCourse userCourse = new UserCourse()
                {
                    UserId = user.Id,
                    CourseId = course.Id
                };
                _context.UserCourses.Add(userCourse);
                course.Capacity = course.Capacity != null ? course.Capacity - 1 : (int?)null;
                _context.SaveChanges();
                return RedirectToAction("UserCourse","Account");

            }

            var oldFactor = _context.Factors.SingleOrDefault(p => p.IsFinally != true && p.UserId == user.Id);
            if (oldFactor != null)
            {
                updateCoursePrice(oldFactor.CourseId);
                oldFactor.CourseId = course.Id;
                oldFactor.FactorDate = DateTime.Now;
                oldFactor.TotalPrice = (decimal)course.TotalPrice;
                oldFactor.UserId = user.Id;
                course.Capacity = course.Capacity != null ? course.Capacity - 1 : (int?)null;
                _context.SaveChanges();
                return RedirectToAction("ShowFactor", "Payment", new { id = oldFactor.Id });
            }
            Factor newFactor = new Factor()
            {
                CourseId = course.Id,
                CourseTotalPrice = (decimal)course.TotalPrice,
                FactorDate = DateTime.Now,
                IsFinally = false,
                TotalPrice = (decimal)course.TotalPrice,
                UserId = user.Id,
            };
            course.Capacity = course.Capacity != null ? course.Capacity - 1 : (int?)null;
            _context.Factors.Add(newFactor);
            _context.SaveChanges();
            return RedirectToAction("ShowFactor", "Payment", new { id = newFactor.Id });
        }

        public IActionResult ShowFactor(int id)
        {
            updateFactor(id);
            var Factor = _context.Factors.Include(p => p.DiscountCode).Include(p => p.Course)
                .Include(p => p.Course.Master).FirstOrDefault(d => d.Id == id);

            if (Factor == null)
            {
                return BadRequest();
            }

            return View(Factor);
        }
        public IActionResult ApplyDiscount(int factorId, string discountCode)
        {
            if (string.IsNullOrEmpty(discountCode))
            {
                return Json(new { success = false, message = "کد تخفیف وارد نشده است!" });
            }

            var factor = _context.Factors
                .Include(f => f.Course)
                .FirstOrDefault(f => f.Id == factorId);

            if (factor == null || factor.IsFinally)
            {
                return Json(new { success = false, message = "فاکتور نامعتبر است!" });
            }

            // پیدا کردن کد تخفیف جدید
            var discount = _context.DiscountCodes
                .FirstOrDefault(d => d.CodeValue.ToUpper() == discountCode.ToUpper() && d.IsActive == true);

            if (discount == null)
            {
                return Json(new { success = false, message = "کد تخفیف نامعتبر یا منقضی شده است!" });
            }

            if (discount.ExpireDate < DateTime.Now)
            {
                return Json(new { success = false, message = "این کد منقضی شده است" });
            }
            if (discount.MaxUsage <= 0)
            {
                return Json(new { success = false, message = "تعداد مجاز استفاده از این کد به اتمام رسیده است" });
            }

            if (discount.Minimum != null && factor.CourseTotalPrice < discount.Minimum)
            {
                return Json(new
                {
                    success = false,
                    message = "حداقل مبلغ مجاز برای استفاده از این کد تخفیف " + discount.Minimum?.ToString("#,0.##") + " تومان است."
                });
            }

            if (discount.Maximum != null && discount.Maximum < factor.CourseTotalPrice)
            {
                return Json(new
                {
                    success = false,
                    message = "حداکثر مبلغ مجاز این کد تخفیف " + discount.Maximum?.ToString("#,0.##") + " تومان است."
                });
            }


            decimal discountAmount = 0;
            decimal newPrice = factor.CourseTotalPrice;

            // حذف تخفیف قبلی در صورت وجود
            if (factor.DiscountCodeId != null)
            {
                var oldDiscount = _context.DiscountCodes.FirstOrDefault(d => d.Id == factor.DiscountCodeId);
                if (oldDiscount != null)
                {
                    if (oldDiscount.DiscountPercent != null)
                    {
                        newPrice = factor.CourseTotalPrice; // بازگرداندن قیمت به مقدار اولیه قبل از تخفیف
                    }
                    else if (oldDiscount.FixedValue != null)
                    {
                        newPrice += (decimal)oldDiscount.FixedValue;
                    }
                }
            }

            // اعمال تخفیف جدید
            if (discount.DiscountPercent != null)
            {
                discountAmount = (decimal)(factor.CourseTotalPrice * discount.DiscountPercent) / 100;
                newPrice = factor.CourseTotalPrice - discountAmount;
            }
            else if (discount.FixedValue != null)
            {
                discountAmount = (decimal)discount.FixedValue;
                newPrice = factor.CourseTotalPrice - discountAmount;
            }

            // به‌روزرسانی فاکتور با کد تخفیف جدید
            factor.DiscountCodeId = discount.Id;
            factor.TotalPrice = newPrice;
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                discountAmount = discountAmount,
                discountPercent = discount.DiscountPercent,
                newPrice = newPrice
            });
        }

        public async Task<IActionResult> NewPayment(int id)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
            if (user == null)
            {
                return BadRequest();
            }

            var factor = _context.Factors.Include(c => c.Course).FirstOrDefault(p => p.Id == id);
            if (factor == null || factor.UserId != user.Id || factor.IsFinally == true)
            {
                return BadRequest();
            }
            updateFactor(factor.Id);
            var url = "https://sandbox.zarinpal.com/pg/v4/payment/request.json"; // آدرس API

            var requestData = new
            {
                merchant_id = "1344b5d4-0048-11e8-94db-005056a205be",
                amount = (int)factor.TotalPrice * 10, // تبدیل تومان به ریال
                description = "خرید دوره " + factor.Course.CourseName,
                callback_url = "https://localhost:7208/payment/verify"
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                if (result.data.code == 100)
                {

                    string authority = result.data.authority;
                    string paymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}";
                    Transaction transaction = new Transaction()
                    {
                        ActionDate = DateTime.Now,
                        BankCallBackId = authority,
                        FactorId = factor.Id,
                        Price = factor.TotalPrice,
                        UserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserId = user.Id

                    };
                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();
                    return Redirect(paymentUrl);
                }
            }

            return BadRequest("خطایی در پرداخت رخ داده است.");
        }
        [HttpGet]

        public async Task<IActionResult> verify(string Authority, string Status)
        {
            var factor = _context.Transactions.FirstOrDefault(f => f.BankCallBackId == Authority);
            var findFactor = _context.Factors.Include(p => p.Course).Include(p => p.DiscountCode).FirstOrDefault(d => d.Id == factor.FactorId);
            if (Status != "OK")
            {
                TempData["Cancel"] = "پرداخت لغو شد!";
                return RedirectToAction("ShowFactor", new { id = findFactor.Id });
            }

            var url = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json"; // آدرس API تأیید پرداخت

            if (factor == null)
            {
                return NotFound();
            }

            var requestData = new
            {
                merchant_id = "1344b5d4-0048-11e8-94db-005056a205be",
                amount = (int)factor.Price * 10, // مقدار پرداختی به ریال
                authority = Authority
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                if (result.data.code == 100 || result.data.code == 101) // 100 = موفق، 101 = قبلاً تأیید شده
                {
                    UserCourse classMember = new UserCourse()
                    {
                        UserId = findFactor.UserId,
                        CourseId = findFactor.CourseId,
                    };
                    _context.UserCourses.Add(classMember);

                    findFactor.IsFinally = true;
                    findFactor.Course.Capacity--;
                    findFactor.RefID = result.data.ref_id;
                    if (findFactor.DiscountCode != null)
                    {
                        findFactor.DiscountCode.UsageCount++;
                        findFactor.DiscountCode.MaxUsage--;
                    }
                    _context.SaveChanges();

                    return RedirectToAction("SuccessPayment", new { refId = result.data.ref_id, factorId = findFactor.Id });
                }
                return RedirectToAction("FailedPayment", new { refId = result.data.ref_id, factorId = findFactor.Id });
            }

            return BadRequest();
        }

        public IActionResult SuccessPayment(string refId, int factorId)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
            var transaction = _context.Factors.Include(f => f.Course).FirstOrDefault(t => t.Id == factorId);

            if (transaction == null || transaction.IsFinally != true || transaction.UserId != user.Id)
            {
                return NotFound("تراکنش مورد نظر یافت نشد.");
            }



            ViewBag.FactorId = transaction.Id;
            ViewBag.Amount = transaction.TotalPrice.ToString("#,0.##");
            ViewBag.CourseName = transaction.Course.CourseName;
            ViewBag.Code = transaction.RefID;
            return View();
        }

        public IActionResult FailedPayment(string refId, int factorId)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
            var transaction = _context.Factors.Include(f => f.Course).FirstOrDefault(t => t.Id == factorId);

            if (transaction == null || transaction.IsFinally != true || transaction.UserId != user.Id)
            {
                return NotFound("تراکنش مورد نظر یافت نشد.");
            }



            ViewBag.FactorId = transaction.Id;
            ViewBag.Amount = transaction.TotalPrice.ToString("#,0.##");
            ViewBag.CourseName = transaction.Course.CourseName;
            ViewBag.Code = transaction.RefID;
            return View();
        }

        public void updateCoursePrice(int courseid)
        {
            var course = _context.Courses.Find(courseid);
            if (course.DiscountStatus == true && course.DiscountPercent != 0)
            {
                if (course.DateEnd < DateTime.Now)
                {
                    course.DiscountStatus = false;
                    course.TotalPrice = course.BasicPrice;

                }
                else
                {
                    course.TotalPrice = course.BasicPrice - ((course.BasicPrice * course.DiscountPercent) / 100);
                }


            }
            else
            {
                course.TotalPrice = course.BasicPrice;
            }

            _context.SaveChanges();
        }

        public void updateFactor(int factorid)
        {
            var factor = _context.Factors.Include(f => f.Course).FirstOrDefault(f => f.Id == factorid);


            updateCoursePrice(factor.CourseId);
            var course = _context.Courses.Find(factor.CourseId);
            factor.CourseTotalPrice = (decimal)course.TotalPrice;
            if (factor.DiscountCodeId != null)
            {
                var olddiscountcode = _context.DiscountCodes.Find(factor.DiscountCodeId);
                if (olddiscountcode.IsActive == true && olddiscountcode.MaxUsage > 0 && (olddiscountcode.ExpireDate == null || olddiscountcode.ExpireDate > DateTime.Now))
                {

                    factor.TotalPrice = olddiscountcode.CalculateDiscount(factor.CourseTotalPrice);
                }
                else
                {
                    factor.TotalPrice = (decimal)course.TotalPrice;
                }



            }
            else
            {
                factor.TotalPrice = (decimal)course.TotalPrice;
            }
            _context.SaveChanges();
        }
    }
}
