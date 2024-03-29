﻿using ProjectsHub.Core;
using ProjectsHub.Model;
using ProjectsHub.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;
using Microsoft.Extensions.Hosting;

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

        public async Task<List<CommentReturnDto>> CommentOnPost(string userId, string postId, Chunk comment)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
            {
                Console.WriteLine($"user {userId} not found!");
                throw new Exception();
            }

            var post = await _postRepository.GetAsync(postId);
            if (post == null)
            {
                Console.WriteLine($"post not found {postId}");
                throw new Exception();
            }
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

            var returnComments = await ToCommentReturnDtoList(post.Comments.OrderBy(c => c.CreatedDate).ToList());

            return returnComments;
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

        public async Task<List<CommentReturnDto>> DeleteCommentOnPost(string userId, string postId, int commentId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
                throw new Exception();

            var post = await _postRepository.GetAsync(postId);
            if (post == null)
                throw new Exception();

            var comment = post.Comments.Find(c => c.Id == commentId);
            if (comment == null)
                return new List<CommentReturnDto>();

            if (userId != comment.UserId)
                throw new UserDoesNotHavePermissionException();

            post.Comments.Remove(comment);

            await _postRepository.UpdateAsync(postId, post);

            Console.WriteLine($"user {userId} removed comment {commentId} on post {postId}");
            var returnComments = await ToCommentReturnDtoList(post.Comments.OrderBy(c => c.CreatedDate).ToList());
            return returnComments;
        }

        private async Task<List<CommentReturnDto>> ToCommentReturnDtoList(List<Comment> comments)
        {
            var TaskUsersList = comments.Select(c => _userService.GetUserShortPeofile(c.UserId));
            var userList = await Task.WhenAll(TaskUsersList);
            var retCommentsList = new List<CommentReturnDto>();
            foreach (Comment c in comments)
            {
                var user = userList.Where(u => u._id.Equals(c.UserId)).FirstOrDefault();
                retCommentsList.Add(c.ToCommentReturnDto(user));
            }
            return retCommentsList;
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

        public async Task<List<CommentReturnDto>> GetComments(string postId)
        {
            var post = await _postRepository.GetAsync(postId);
            if (post == null)
                throw new Exception();
            var returnComments = await ToCommentReturnDtoList(post.Comments.OrderBy(c => c.CreatedDate).ToList());
            return returnComments;
        }

        public async Task<ShortPost> GetShortPost(string userId, string postId)
        {
            var post = await _postRepository.GetAsync(postId);
            if (post == null)
                throw new Exception();
            var user = await _userService.GetUserShortPeofile(userId);
            var isFollowed = await _userService.IsFollowing(userId, post.AuthorId);

            var shortPost = post.ToShortPost(user, isFollowed, userId);
            return shortPost;
        }

        public async Task<List<ShortPost>> GetUserPostsList(string loggedInUserId, string userIWantPostsId)
        {
            var postsIds = await _userService.GetUserPosts(userIWantPostsId);

            var isFollowed = await _userService.IsFollowing(loggedInUserId, userIWantPostsId);
            var user = await _userService.GetUserShortPeofile(userIWantPostsId);

            var PostsListTasks = postsIds.Select(p => _postRepository.GetAsync(p));

            var PostsList = await Task.WhenAll(PostsListTasks);

            var shortPostList = PostsList.Select(p => p.ToShortPost(user, isFollowed, loggedInUserId)).ToList();

            return shortPostList;
        }

        public async Task<List<ShortPost>> GetNewsFeedPosts(string id, int pageNo)
        {
            var usersIds = await _userService.GetListOfFollwing(id);

            var postsIdsList = new List<string>();
            foreach ( var user in usersIds)
            {
                var postIds = await _userService.GetUserPosts(user);
                postIds.ForEach(postId => postsIdsList.Add(postId));
            }

            var listOfReturnPosts = await _postRepository.GetAsync(postsIdsList ,pageNo);


            var listOfShortPostsTasksReturn = listOfReturnPosts.Select(async p => p.ToShortPost(await _userService.GetUserShortPeofile(p.AuthorId), true, id)).ToList();

            var listOfShortPostsReturn = await Task.WhenAll(listOfShortPostsTasksReturn);

            return listOfShortPostsReturn.Select(s => s).ToList();
        }

        public async Task<List<ShortPost>> SearchPosts(string id, string query, int pageNo)
        {
            var posts = await _postRepository.SearchAsync(query, pageNo);

            var listOfTaskShortPosts = posts.Select(async p =>
                p.ToShortPost(
                    await _userService.GetUserShortPeofile(p.AuthorId),
                    await _userService.IsFollowing(id, p.AuthorId),
                    id));
            var listOfShortPosts = await Task.WhenAll(listOfTaskShortPosts);
            return listOfShortPosts.Select(s => s).ToList();
        }
    }
}
