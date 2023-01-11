using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public Task<Post> CreatePost(CreatePostDto post, string userId);
        public Task<Post> GetPost(string id);
        public Task DeletePost(string PostId, string userId);
    }
}