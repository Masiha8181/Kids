using Kids.DTO;
using Kids.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Kids.Controllers
{
    public class AccountController : Controller
    {
        private readonly  KidsContext _context;

        public AccountController(KidsContext context)
        {
            _context = context;
        }
        [Route("/Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            return View();
        }


        [Route("/Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return Json(new { success = false, errors });
            //}
            Random r = new Random();
            var Code = r.Next(1000, 9999);
            if (ModelState.IsValid)
            {
                var PhoneNumberConvert = ConvertPersianDigitsToEnglish.NumFa2En(registerDto.PhoneNumber);
                var IsExist = _context.Users.Any(p => p.PhoneNumber == PhoneNumberConvert && p.IsActive == true);
                if (IsExist)
                {
                    ModelState.AddModelError("PhoneNumber", "این شماره موبایل از قبل موجود است");
                    return View();
                }

                var UserNotActive =
                    _context.Users.FirstOrDefault(p => p.PhoneNumber == PhoneNumberConvert && p.IsActive == false);
                if (UserNotActive != null)
                {
                    TimeSpan breakDuration = TimeSpan.FromMinutes(2);
                    if (DateTime.Now - UserNotActive.LastRequest <= breakDuration)
                    {
                        ModelState.AddModelError("PhoneNumber", "لطفا 2 دقیقه دیگر درخواست دهید");
                        return View();
                    }

                    UserNotActive.Name = registerDto.FullName;

                    UserNotActive.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                    UserNotActive.PhoneNumber = PhoneNumberConvert;
                    UserNotActive.ActiveCode = Code;
                    UserNotActive.CreateDate = DateTime.Now;
                    UserNotActive.IsActive = false;
                    UserNotActive.IsAdmin = false;
                    UserNotActive.LastRequest = DateTime.Now;
                    UserNotActive.IsDeleted = false;
                    _context.SaveChanges();



                    var SmsSend = await SmsService.SendSmsAsync(PhoneNumberConvert, Code.ToString());
                    if (SmsSend.Result == true)
                    {
                        TempData["send"] = true;
                        return View("Verify", ViewData["PhoneNumber"] = PhoneNumberConvert);
                    }
                    else
                    {
                        ModelState.AddModelError("PhoneNumber", SmsSend.ResultMessege);
                        return View();

                    }


                }
                else
                {
                    User user = new User()
                    {
                        Name = registerDto.FullName,
                    
                        Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                        PhoneNumber = PhoneNumberConvert,
                        ActiveCode = Code,
                        CreateDate = DateTime.Now,
                        IsActive = false,
                        IsAdmin = false,
                        LastRequest = DateTime.Now,
                        IsDeleted = false

                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    var SmsSend = await SmsService.SendSmsAsync(PhoneNumberConvert, Code.ToString());
                    if (SmsSend.Result == true)
                    {
                        TempData["send"] = true;
                        return View("Verify", ViewData["PhoneNumber"] = PhoneNumberConvert);

                    }
                    else
                    {
                        ModelState.AddModelError("PhoneNumber", SmsSend.ResultMessege);
                        return View();
                    }




                }

                return View();
            }

            return View();
        }
        [Route("Verify")]
        [HttpPost]
        public async Task<IActionResult> Verify(string PhoneNumber, int? Code)
        {
            var NormalCctiveCode = ConvertPersianDigitsToEnglish.NumFa2En(Code.ToString());
            var User = _context.Users.FirstOrDefault(p =>
                p.PhoneNumber == PhoneNumber && p.ActiveCode == int.Parse(NormalCctiveCode));
            if (User == null)
            {
                ViewBag.Error = 1;
                return View(ViewData["PhoneNumber"] = PhoneNumber);
            }
            TimeSpan breakDuration = TimeSpan.FromMinutes(2);
            if (DateTime.Now - User.LastRequest > breakDuration)
            {
                ViewBag.Error = 2;
                return View(ViewData["PhoneNumber"] = PhoneNumber); ;

            }

            if (User.IsActive == true)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,User.Name),
                    new Claim("PhoneNumber", User.PhoneNumber),
                    new Claim("IsAdmin", User.IsAdmin.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties()
                {

                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) //
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return RedirectToAction("DashBoard");
            }

            if (User.IsActive == false)
            {
                Random r = new Random();
                User.IsActive = true;
                User.ActiveCode = r.Next(1000, 9999);

                _context.SaveChanges();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, User.Name),
                    new Claim("PhoneNumber", User.PhoneNumber)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties()
                {

                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) //
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }


            TempData["ToastrMessage"] = "ورود موفق!";
            return RedirectToAction("DashBoard");
        }

        [Route("ResendCode")]
        [HttpPost]
        public async Task<IActionResult> ResendCodeAsync([FromBody] string PhoneNumber)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == PhoneNumber);
            if (user == null)
            {
                return Json(new { success = false, message = "کاربر یافت نشد." });
            }

            TimeSpan breakDuration = TimeSpan.FromMinutes(2);
            if (DateTime.Now - user.LastRequest <= breakDuration)
            {
                return Json(new { success = false, message = "لطفا دو دقیقه دیگر درخواست دهید" });

            }


            string newCode = new Random().Next(1000, 9999).ToString();
            user.ActiveCode = int.Parse(newCode);
            user.LastRequest = DateTime.Now;
            _context.SaveChanges();
            var SmsSend = await SmsService.SendSmsAsync(PhoneNumber, newCode.ToString());
            if (SmsSend.Result == true)
            {
                return Json(new { success = true, message = "کد تأیید ارسال شد." });
            }
            else if (SmsSend.Result == false)
            {
                ModelState.AddModelError("PhoneNumber", SmsSend.ResultMessege);
                return Json(new { success = false, message = "خطا در ارسال پیامک." });
            }

            Console.WriteLine($"کد تأیید جدید برای {user.PhoneNumber}: {newCode}");

            return Json(new { success = true, message = "کد تأیید ارسال شد." });
        }
        [Route("/Logout")]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            return View();
        }
        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            if (ModelState.IsValid)
            {
                var phonenum = ConvertPersianDigitsToEnglish.NumFa2En(loginDto.PhoneNumber);
                var User = _context.Users.FirstOrDefault(p =>
                    p.PhoneNumber == phonenum && p.IsActive == true && p.IsDeleted != true);
                if (User == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, User.Password))
                {
                    ModelState.AddModelError("PhoneNumber", "اطلاعات وارد شده یافت نشد");
                    return View();
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,User.Name),
                    new Claim("PhoneNumber", User.PhoneNumber),
                    new Claim("IsAdmin", User.IsAdmin.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties()
                {

                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) //
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["ToastrMessage"] = "ورود موفق!";
                return RedirectToAction("DashBoard", "Account");
                
            }

            return View();
        }

        [Route("LoginWithCode")]
        public IActionResult LoginWithCode()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            return View();
        }

        [HttpPost]
        [Route("LoginWithCode")]
        public async Task<IActionResult> LoginWithCodeAsync(LoginWithCodeDTO verifyDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashBoard", "Account");

            }
            if (ModelState.IsValid)
            {
                var phonenum = ConvertPersianDigitsToEnglish.NumFa2En(verifyDto.PhoneNumber);
                var User = _context.Users.FirstOrDefault(p =>
                    p.IsActive == true && p.PhoneNumber == phonenum&&p.IsDeleted!=true);
                if (User == null)
                {
                    ModelState.AddModelError("PhoneNumber", "کاربری یافت نشد");
                    return View();
                }

                TimeSpan breakDuration = TimeSpan.FromMinutes(2);
                if (DateTime.Now - User.LastRequest <= breakDuration)
                {
                    ModelState.AddModelError("PhoneNumber", "لطفا 2 دقیقه دیگر درخواست دهید");
                    return View();

                }
                string newCode = new Random().Next(1000, 9999).ToString();
                User.ActiveCode = int.Parse(newCode);
                User.LastRequest = DateTime.Now;
                _context.SaveChanges();
                var SmsSend = await SmsService.SendSmsAsync(phonenum, newCode.ToString());
                if (SmsSend.Result == true)
                {
                    TempData["send"] = true;
                    return View("Verify", ViewData["PhoneNumber"] = verifyDto.PhoneNumber);
                }
                else if (SmsSend.Result == false)
                {
                    ModelState.AddModelError("PhoneNumber", SmsSend.ResultMessege);
                    return View();
                }
            }

            return View();
        }
        [Authorize]
        [Route("/DashBoard")]
        
        public IActionResult DashBoard()
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);
            if (user == null)
            {
                return NotFound();
            }

            var UserDTO = new ShowUserInformationDTO()
            {
                Name =user.Name,
                BirthDate = user.BirthDate,
                CreateDate = user.CreateDate,
                Email = user.Email,
                HomeNumber = user.HomeNumber,
                PhoneNumber = user.PhoneNumber,
                Sex = user.Sex,
                ImageAddress = user.ImageAddress
            };
            return View(UserDTO);
        }
    
        public IActionResult SideDashBoard()
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);
            if (user==null)
            {
                return NotFound();
            }

            if (user.ImageAddress!=null)
            {
                ViewBag.ImageAddress = user.ImageAddress;
            }
            return PartialView();
        }
        [Authorize]
        [Route("/EditProfile")]
        public IActionResult EditProfile()
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);
            if (user == null)
            {
                return NotFound();
            }
            var UserDTO = new EditProfileDTO()
            {
                Name = user.Name,
                BirthDate = user.BirthDate,
                Email = user.Email,
                HomeNumber = user.HomeNumber,
                Sex = user.Sex,
                ImageAddress = user.ImageAddress
            };
            return View(UserDTO);
        }
        [Authorize]
        [Route("/EditProfile")]
        [HttpPost]
        [HttpPost]
        public IActionResult EditProfile(EditProfileDTO editProfileDto, IFormFile? Image,DateTime? DiscountDate)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // اگر عکس جدیدی آپلود شده بود
                if (Image != null)
                {
                    var currentDir = Directory.GetCurrentDirectory();
                 
                    var uploadFolder = Path.Combine(currentDir, "wwwroot", "Uploads", "UserImage");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    // حذف عکس قدیمی اگر وجود داشت
                    if (!string.IsNullOrEmpty(user.ImageAddress))
                    {
                        var oldImagePath = Path.Combine(uploadFolder, user.ImageAddress);
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // ذخیره عکس جدید
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var newImagePath = Path.Combine(uploadFolder, newFileName);

                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    user.ImageAddress = newFileName;
                }

                // بروزرسانی اطلاعات دیگر
                user.Name = editProfileDto.Name;
            
                user.Email = editProfileDto.Email;
                user.HomeNumber = editProfileDto.HomeNumber;
                user.Sex = editProfileDto.Sex;
                if (DiscountDate!=null)
                {
                    user.BirthDate = DiscountDate;
                }
                _context.SaveChanges();

                TempData["SuccessMessage"] = "پروفایل با موفقیت ویرایش شد.";
                return RedirectToAction("EditProfile");
            }

            return View(editProfileDto);
        }
        [Authorize]
        [Route("/ChangePhoneNumber")]
        public IActionResult ChangePhoneNumber()
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();

            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/SendCode")]
     
        public async Task<IActionResult> SendCode(string phoneNumber)
        {
            var userPhoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == userPhoneNumber && p.IsActive && !p.IsDeleted);

            if (user == null)
                return Json(new { success = false, message = "کاربر یافت نشد." });

            string phone = ConvertPersianDigitsToEnglish.NumFa2En(phoneNumber);

            // بررسی تکراری بودن شماره
            bool isExist = _context.Users.Any(p => p.PhoneNumber == phone && p.IsActive);
            if (isExist)
            {
                return Json(new { success = false, message = "این شماره موبایل متعلق به کاربر دیگری است." });
            }

            // محدودیت زمانی برای ارسال کد
            TimeSpan breakDuration = TimeSpan.FromMinutes(2);
            if (DateTime.Now - user.LastRequest <= breakDuration)
            {
                return Json(new { success = false, message = "لطفاً ۲ دقیقه دیگر تلاش کنید." });
            }

            // تولید و ذخیره کد
            Random r = new Random();
            int code = r.Next(1000, 9999);
            user.ActiveCode = code;
            user.LastRequest = DateTime.Now;
            _context.SaveChanges();

            var smsResult = await SmsService.SendSmsAsync(phone, code.ToString());

            if (smsResult.Result == true)
            {
                return Json(new { success = true, message = "کد با موفقیت ارسال شد." });
            }
            else
            {
                return Json(new { success = false, message = smsResult.ResultMessege });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/VerifyCode")]
        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code, string oldPhoneNumber, string newPhoneNumber)
        {
            var NormalCctiveCode = ConvertPersianDigitsToEnglish.NumFa2En(code.ToString());
            string cleanOldPhone = ConvertPersianDigitsToEnglish.NumFa2En(oldPhoneNumber);
            string cleanNewPhone = ConvertPersianDigitsToEnglish.NumFa2En(newPhoneNumber);

            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == cleanOldPhone && p.IsActive && !p.IsDeleted);

            if (user == null)
                return Json(new { success = false, message = "کاربر یافت نشد." });

            if (user.ActiveCode != int.Parse(code))
            {
                return Json(new { success = false, message = "کد وارد شده اشتباه است." });
            }

            // شماره جدید تکراری نباشه
            bool isExist = _context.Users.Any(p => p.PhoneNumber == cleanNewPhone && p.IsActive && !p.IsDeleted);
            if (isExist)
            {
                return Json(new { success = false, message = "این شماره قبلاً ثبت شده است." });
            }
            Random r = new Random();
            int NewCode = r.Next(1000, 9999);
            // تغییر شماره موبایل
            user.PhoneNumber = cleanNewPhone;
            user.ActiveCode = NewCode;
            user.LastRequest=DateTime.Now;// پاک کردن کد پس از استفاده
            _context.SaveChanges();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim("PhoneNumber", newPhoneNumber),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {

                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7) //
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Json(new { success = true, message = "شماره موبایل با موفقیت تغییر کرد." });
        }
        [Authorize]
        [HttpPost]
        [Route("/ResendNewCode")]
        public async Task<IActionResult>  ResendCode(string OldPhoneNumber,string NewPhoneNumber)
            {
            string cleanOldPhone = ConvertPersianDigitsToEnglish.NumFa2En(OldPhoneNumber);
            string cleanNewPhone = ConvertPersianDigitsToEnglish.NumFa2En(NewPhoneNumber);

            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == cleanOldPhone && p.IsActive && !p.IsDeleted);

            if (user == null)
                return Json(new { success = false, message = "کاربر یافت نشد." });

            TimeSpan breakDuration = TimeSpan.FromMinutes(2);
            if (DateTime.Now - user.LastRequest <= breakDuration)
            {
                return Json(new { success = false, message = "لطفاً ۲ دقیقه دیگر تلاش کنید." });
            }
            Random r = new Random();
            int NewCode = r.Next(1000, 9999);
            
          
            user.ActiveCode = NewCode;
            user.LastRequest=DateTime.Now;
            _context.SaveChanges();

            var smsResult = await SmsService.SendSmsAsync(cleanNewPhone, NewCode.ToString());

            if (smsResult.Result == true)
            {
                return Json(new { success = true, message = "کد با موفقیت ارسال شد." });
            }
            else
            {
                return Json(new { success = false, message = smsResult.ResultMessege });
            }
            return Json(new { success = true, message = "کد مجدد ارسال شد" });
        }
        [Route("/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [Route("/ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.Password);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "با موفقیت ویرایش شد";
            }
            return View();
        }

        [Authorize]
        [Route("/UserCourse")]
        public IActionResult UserCourse()
        {

            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();
            var courselist = _context.UserCourses.Where(p => p.UserId == user.Id).Include(p => p.Course.Master).Include(p => p.Course)
                .ThenInclude(p => p.Episodes).ToList();

            return View(courselist);
        }
        [Authorize]
        [Route("/UserFactor")]
      
        public IActionResult UserFactor()
        {
            var phoneNumber = User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
            var user = _context.Users.SingleOrDefault(p =>
                p.PhoneNumber == phoneNumber && p.IsActive == true && p.IsDeleted != true);

            if (user == null)
                return NotFound();
            var factors = _context.Factors.Where(p => p.UserId == user.Id).Include(c => c.Course).Include(p => p.DiscountCode).ToList();
            return View(factors);
        }

    }
}
