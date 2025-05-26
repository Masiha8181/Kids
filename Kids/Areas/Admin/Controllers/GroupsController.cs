using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.AdminDTO;
using Kids.Areas.Admin.Attribute;

namespace Kids.Areas.Admin.Controllers
{
  
    [Admin]
    [Area("Admin")]
    public class GroupsController : Controller
    {
        private readonly KidsContext _context;

        public GroupsController(KidsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            return View(list);
        }

        public IActionResult Create()
        {

            var list = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>  Create( CreateGroupsDTO createGroupsDto)
        {
           
            if (ModelState.IsValid)
            {
                var uniqueFileName="";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/GroupsImage");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (createGroupsDto.ImageAddress!=null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createGroupsDto.ImageAddress.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createGroupsDto.ImageAddress.CopyToAsync(stream);
                    }
                    var imagePath = "/uploads/GroupsImage" + uniqueFileName;
                }



              
                var list = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
                ViewBag.List = list;
                CourseGroup courseGroup = new CourseGroup()
                {
                    GroupName = createGroupsDto.GroupName,
                    ParentId = createGroupsDto.ParentID,
                    ImageAddress = uniqueFileName,
                    Descreption = createGroupsDto.Descreption
                };
                _context.CourseGroups.Add(courseGroup);
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ثبت شد";
                return View();
            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.CourseGroups.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "آیتم یافت نشد!" });
            }

            if (item.ImageAddress != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/GroupsImage");
                var GroupsImagePath = item.ImageAddress;
                var filePath = Path.Combine(uploadsFolder, GroupsImagePath);
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
            var list = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
            ViewBag.List = list;
            var Groups = _context.CourseGroups.Find(id);
            EditGroupsDTO editGroupsDto = new EditGroupsDTO()
            {
                GroupName = Groups.GroupName,
                Id = Groups.Id,
                ParentID = Groups.ParentId,
                ImageAddress = Groups.ImageAddress,
                Descreption = Groups.Descreption,
                
            };
            return View(editGroupsDto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditGroupsDTO editGroupsDto,IFormFile? ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var Group = _context.CourseGroups.Find(editGroupsDto.Id);
                var finalImageAddress = Group.ImageAddress;
                if (ImageUpload != null)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/GroupsImage");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }


                    var pastFileName = Group.ImageAddress;
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

                Group.ImageAddress = finalImageAddress;
                Group.GroupName = editGroupsDto.GroupName;
                Group.ParentId = editGroupsDto.ParentID;
                Group.Descreption=editGroupsDto.Descreption;
                _context.SaveChanges();
                TempData["Success"] = "با موفقیت ویرایش شد";
                var list = _context.CourseGroups.Where(p => p.IsDeleted != true).ToList();
                ViewBag.List = list;
                return View(editGroupsDto);
            }

            return View(editGroupsDto); ;
        }
    }
}
