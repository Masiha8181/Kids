using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kids.Areas.Admin.Attribute;
using Kids.DTO;
using Kids.Repository.Interface;


namespace Kids.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    [Admin]
    public class CommentController : Controller
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }
        public IActionResult Index()
        {
            var list = commentService.getList();
            return View(list);
        }
        public IActionResult Edit(int id)
        {
            var comment = commentService.Get(id);
            CommentDTO.EditCommentDTO editCommentDto = new CommentDTO.EditCommentDTO()
            {
                CommentText = comment.CommentText,
                IsApproved = comment.IsApproved,
                Id = comment.Id
            };

            return View(editCommentDto);
        }
        [HttpPost]
        public IActionResult Edit(CommentDTO.EditCommentDTO editCommentDto)
        {
            if (ModelState.IsValid)
            {
              var result = commentService.Edit(editCommentDto);
              if (result)
              {
                  TempData["Success"] = "با موفقیت ویرایش شد";
                  return View(editCommentDto);
                }
            }

            return View(editCommentDto);
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
         var result=   commentService.Delete(id);
            if (result!=true)
            {
                return Json(new { success = false, message = " خطا!" });
            }
            return Json(new { success = true, message = "حذف شد!" });
        }
        [HttpPost]
        public IActionResult Approved(int id)
        {
            var result = commentService.Approved(id);
            if (result == false)
            {
                return Json(new { success = false, message = "خظا!" });
            }

         

            return Json(new { success = true });
        }
    }
    
    }
