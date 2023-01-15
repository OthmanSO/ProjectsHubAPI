using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public static class CommentHelpers
    {
        public static CommentReturnDto ToCommentReturnDto(this Comment comment, UserShortProfileDto user)
        {
            return new CommentReturnDto
            {
                Id= comment.Id,
                Commentchunk= comment.Commentchunk,
                CreatedDate= comment.CreatedDate,
                User = user
            };
        }
    }
}
