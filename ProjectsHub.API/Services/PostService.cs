using ProjectsHub.Core;
using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository PostRepository;
        public PostService()
        {

        }

        public void CreatePost(CreatePostDto post, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
