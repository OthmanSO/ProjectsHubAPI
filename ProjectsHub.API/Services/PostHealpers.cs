using ProjectsHub.Model;
using System.Runtime.CompilerServices;

namespace ProjectsHub.API.Services
{
    public static class PostHealpers
    {
        public static PostReturnDto ToPostReturnDto(this ReturnPostDto post) => new PostReturnDto
        {
            _id = post._id,
            Title = post.Title,
            CreatedDate = post.CreatedDate,
            CoverPicture = post.CoverPicture,
            AuthorId = post.AuthorId,
            UsersWhoLiked = post.UsersWhoLiked.Count,
            PostChunks = post.PostChunks,
            Comments = post.Comments
        };

        public static void FromCreatePostDto(this ReturnPostDto post, CreatePostDto createPost) 
        {
            post.PostChunks = createPost.PostChunks;
            post.Comments = new List<Comment>();
            post.CoverPicture = createPost.CoverPicture;
            post.CreatedDate = System.DateTime.Now;
            post.Title = createPost.Title;
            post.UsersWhoLiked = new List<string>();
        }
    }
}

