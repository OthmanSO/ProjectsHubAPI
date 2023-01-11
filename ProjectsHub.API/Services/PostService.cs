using ProjectsHub.Core;
using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly UserService _userService;
        public PostService(IPostRepository postRepository, UserService userService)
        {
            this._postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Post> CreatePost(CreatePostDto post, string userId)
        {
            //if doesnot exist throw exception 
            _userService.GetUserProfileById(Guid.Parse(userId));
            Post createPost = new Post
            {
                AuthorId = userId,
                PostChunks = post.PostChunks,
                Comments = new List<Comment>(),
                CoverPicture = post.CoverPicture,
                CreatedDate = System.DateTime.Now,
                Title = post.Title,
                UsersWhoLiked = new List<string>()
            };
            return await _postRepository.CreateAsync(createPost);
        }
        public async Task<Post> GetPost(string id)
        {
            return await _postRepository.GetAsync(id);
        }
    }
}
