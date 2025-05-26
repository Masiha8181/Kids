using Kids.Context;
using Kids.DTO;
using Kids.Models;

namespace Kids.Repository.Interface
{

    public interface ICommentService
    {

        public Comment Get(int id);
        bool Create(CommentDTO.CreateCommentDTO comment);
        bool Edit(CommentDTO.EditCommentDTO comment);
        bool Delete(int id);
        bool Approved(int id);
        List<Comment> getList();
        List<Comment> GetCourseComments(int PageId, int CourseId);
    }
}
