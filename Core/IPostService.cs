using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public Task<PostReturnDto> CreatePost(CreatePostDto post, string userId);
        public Task<PostReturnDto> GetPost(string userid, string postId);
        public Task DeletePost(string PostId, string userId);
        public Task LikePost(string userId, string postId);
        public Task UnLikePost(string userId, string postId);
    }
}