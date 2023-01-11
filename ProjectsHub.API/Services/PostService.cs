using ProjectsHub.Core;
using ProjectsHub.Model;
using ProjectsHub.Exceptions;

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

        public async Task<ReturnPostDto> CreatePost(CreatePostDto post, string userId)
        {
            //if doesnot exist throw exception 
            _userService.GetUserProfileById(Guid.Parse(userId));
            ReturnPostDto createPost = new ReturnPostDto();
            createPost.FromCreatePostDto(post);
            createPost.AuthorId = userId;
            return await _postRepository.CreateAsync(createPost);
        }

        public async Task DeletePost(string postId, string userId)
        {
            var post = await _postRepository.GetAsync(postId);
            if (post.AuthorId != userId)
            {
                throw new UserDoesNotHavePermissionException();
            }
            _postRepository.RemoveAsync(postId);
        }


        public async Task<PostReturnDto> GetPost(string id)
        {
            var post = await _postRepository.GetAsync(id);
            return post.ToPostReturnDto();
        }
    }
}
