using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetAsync();
        public Task<Post> GetAsync(string id);
        public Task<List<Post>> GetByAuthorAsync(string id);
        public Task<Post> CreateAsync(Post newPost);
        public Task UpdateAsync(string id, Post updatedPost);
        public Task RemoveAsync(string id);
        public Task<List<Post>> GetAsync(List<string> postIdsList, int pageNo);
        public Task<List<Post>> SearchAsync(string query, int pageNo);
    }
}
