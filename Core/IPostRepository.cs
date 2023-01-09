using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetAsync();
        public Task<Post> GetAsync(string id);
        public Task<List<Post>> GetByAuthorAsync(string id);
        public Task<Post> CreateAsync(Post newBook);
        public Task UpdateAsync(string id, Post updatedBook);
        public Task RemoveAsync(string id);
    }
}
