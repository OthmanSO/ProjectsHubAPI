using ProjectsHub.Core;
using ProjectsHub.Model;
using ProjectsHub.Exceptions;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<List<Comment>> CommentOnPost(string userId, string postId, Chunk comment)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null) throw new Exception();

            var post = await _postRepository.GetAsync(postId);
            if (post == null) throw new Exception();

            post.Comments = post.Comments ?? new List<Comment>();

            int lastComment = post.Comments.GetLastComment();

            Comment createComment = new Comment
            {
                Id = lastComment + 1,
                UserId = userId,
                Commentchunk = comment,
                CreatedDate = DateTime.UtcNow
            };

            post.Comments.Add(createComment);

            await _postRepository.UpdateAsync(postId, post);

            Console.WriteLine($"user {userId} Commented on post {post}, comment Id = {createComment.Id}");

            return post.Comments.OrderBy(c => c.CreatedDate).ToList();
        }

        public async Task<PostReturnDto> CreatePost(CreatePostDto post, string userId)
        {
            //if doesnot exist throw exception 
            var user = await _userService.GetUserProfileById(userId);

            var createPost = new Post();
            createPost.FromCreatePostDto(post);
            createPost.AuthorId = userId;

            var createdPost = (await _postRepository.CreateAsync(createPost)).ToPostReturnDto();

            await _userService.AddPost(userId, createdPost._id);

            Console.WriteLine($"user {userId} created a post {createdPost._id}");

            return createdPost;
        }

        public async Task<List<Comment>> DeleteCommentOnPost(string userId, string postId, int commentId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)  
                throw new Exception();  

            var post = await _postRepository.GetAsync(postId);
            if (post == null) 
                throw new Exception();

            var comment = post.Comments.Find(c => c.Id == commentId);
            if (comment == null) 
                return new List<Comment>();

            if (userId != comment.UserId)
                throw new UserDoesNotHavePermissionException();

            post.Comments.Remove(comment);

            _postRepository.UpdateAsync(postId, post);

            Console.WriteLine($"user {userId} removed comment {commentId} on post {postId}");
            return post.Comments.ToList();
        }

        public async Task DeletePost(string postId, string userId)
        {
            var post = await _postRepository.GetAsync(postId);
            if (post.AuthorId != userId)
            {
                throw new UserDoesNotHavePermissionException();
            }
            await _userService.RemovePost(userId, postId);
            await _postRepository.RemoveAsync(postId);

            Console.WriteLine($"user {userId} deleted his post {postId}");
        }


        public async Task<PostReturnDto> GetPost(string userId, string postId)
        {
            var post = await _postRepository.GetAsync(postId);
            return post.ToPostReturnDto(userId);
        }

        public async Task LikePost(string userId, string postId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
                throw new Exception();

            var post = await _postRepository.GetAsync(postId);
            if (post == null) throw new Exception();

            if (post.UsersWhoLiked.IsNullOrEmpty())
            {
                post.UsersWhoLiked = new List<string>();
            }
            if (!post.UsersWhoLiked.Any(x => x.Equals(userId)))
            {
                post.UsersWhoLiked.Add(userId);

                Console.WriteLine($"user {userId} now likes post {postId}");
            }

            await _postRepository.UpdateAsync(postId, post);
        }

        public async Task UnLikePost(string userId, string postId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
                throw new Exception();

            var post = await _postRepository.GetAsync(postId);
            if (post == null) throw new Exception();

            if (post.UsersWhoLiked.IsNullOrEmpty() || !post.UsersWhoLiked.Any(x => x.Equals(userId)))
            {
                return;
            }
            post.UsersWhoLiked.Remove(userId);

            Console.WriteLine($"user {userId} unliked post {postId}");

            await _postRepository.UpdateAsync(postId, post);
        }
    }
}
