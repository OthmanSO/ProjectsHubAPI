using ProjectsHub.Model;
using System.Runtime.CompilerServices;

namespace ProjectsHub.API.Services
{
    public static class PostHealpers
    {
        public static PostReturnDto ToPostReturnDto(this Post post,string userId = "") => new PostReturnDto
        {
            _id = post._id,
            Title = post.Title,
            CreatedDate = post.CreatedDate,
            CoverPicture = post.CoverPicture,
            AuthorId = post.AuthorId,
            UsersWhoLiked = post.UsersWhoLiked.Count,
            PostChunks = post.PostChunks,
            Comments = post.Comments,
            IsLiked = post.UsersWhoLiked.Any(user => user.Equals(userId))
        };

        public static void FromCreatePostDto(this Post post, CreatePostDto createPost) 
        {
            post.PostChunks = createPost.PostChunks;
            post.Comments = new List<Comment>();
            post.CoverPicture = createPost.CoverPicture;
            post.CreatedDate = System.DateTime.Now;
            post.Title = createPost.Title;
            post.UsersWhoLiked = new List<string>();
        }
        public static UserShortProfileDto ToUserShortProfileDto(this UserAccount user) =>
            new UserShortProfileDto
            {
                _id = user._Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePic = user.ProfilePicture
            };
}
}

