using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public Task<PostReturnDto> CreatePost(CreatePostDto post, string userId);
        public Task<PostReturnDto> GetPost(string id);
        public Task DeletePost(string PostId, string userId);
    }
}