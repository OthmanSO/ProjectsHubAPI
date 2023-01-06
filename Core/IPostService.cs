using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public void CreatePost(CreatePostDto post, Guid userId);
    }
}