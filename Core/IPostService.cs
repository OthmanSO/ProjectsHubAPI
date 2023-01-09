using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public Task<Post> CreatePost(CreatePostDto post, string userId);
    }
}