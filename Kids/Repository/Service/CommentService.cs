using Microsoft.EntityFrameworkCore;
using Kids.Context;
using Kids.Models;
using Kids.Areas.Admin.Controllers;
using Kids.DTO;
using Kids.Models;
using Kids.Repository.Interface;

namespace Kids.Repository.Service
{
    public class CommentService:ICommentService
    {
        private readonly KidsContext _context;
        private readonly IHttpContextAccessor userAccessor;

        public CommentService(KidsContext context, IHttpContextAccessor userAccessor)
        {
            _context = context;
            this.userAccessor = userAccessor;
        }
        public Comment Get(int id)
        {
           
            var comment = _context.Comments.Find(id);
            if (comment!=null)
            {
                return comment;
            }
            return null;
        }

        public bool Create(CommentDTO.CreateCommentDTO commentDTO)
        {
            if (userAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var phoneNumber = userAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value;
                var user = _context.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber && p.IsActive == true);
                if (user != null&&user.IsActive==true)
                {
                    Comment comment = new Comment()
                    {
                        CourseId = commentDTO.CourseId,
                        CommentText = commentDTO.CommentText,
                        CreateTime = DateTime.Now,
                        IsApproved = false,
                        IsDeleted = false,
                        ParentId = commentDTO.ParentId,
                        UserId =user.Id,
                    };
                    _context.Comments.Add(comment);
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }

            return false;


        }

        public bool Edit(CommentDTO.EditCommentDTO commentDTO)
        {
            var comment = Get(commentDTO.Id);
            if (comment!=null)
            {
                comment.CommentText = commentDTO.CommentText;
              
                comment.IsApproved = commentDTO.IsApproved;
               
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool Delete(int id)
        {
            var comment = Get(id);
            if (comment != null)
            {
                comment.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Approved(int id)
        {
            var comment= Get(id);
            if (comment!=null&&comment.IsDeleted!=true)
            {
                comment.IsApproved = true;
                _context.SaveChanges();
                return true;

            }
            return false;
        }

        public List<Comment> getList()
        {
            var comments = _context.Comments.Include(p=>p.Course).Include(p=>p.User).Where(p => p.IsDeleted != true).Include(p=>p.Article).ToList();
            return comments;
        }

        public List<Comment> GetCourseComments(int PageId, int CourseId)
        {
            return _context.Comments
                .Where(c => c.CourseId == CourseId && c.IsApproved==true&&c.IsDeleted!=true)
                .OrderByDescending(c => c.CreateTime)
                .Skip((PageId - 1) * 10)
                .Take(10)
                .ToList();
        }
    }
}
