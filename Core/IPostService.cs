﻿using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostService
    {
        public Task<PostReturnDto> CreatePost(CreatePostDto post, string userId);
        public Task<PostReturnDto> GetPost(string userid, string postId);
        public Task DeletePost(string PostId, string userId);
        public Task LikePost(string userId, string postId);
        public Task UnLikePost(string userId, string postId);
        public Task<List<CommentReturnDto>> CommentOnPost(string userId, string postId, Chunk comment);
        public Task<List<CommentReturnDto>> DeleteCommentOnPost(string userId, string postId, int commentId);
        public Task<List<CommentReturnDto>> GetComments(string postid);
        public Task<ShortPost> GetShortPost(string userId, string postId);
        public Task<List<ShortPost>> GetUserPostsList(string id, string userId);
        public Task<List<ShortPost>> GetNewsFeedPosts(string id, int pageNo);
        public Task<List<ShortPost>> SearchPosts(string id, string query, int pageNo);
    }
}