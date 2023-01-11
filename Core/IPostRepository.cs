using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IPostRepository
    {
        public Task<List<ReturnPostDto>> GetAsync();
        public Task<ReturnPostDto> GetAsync(string id);
        public Task<List<ReturnPostDto>> GetByAuthorAsync(string id);
        public Task<ReturnPostDto> CreateAsync(ReturnPostDto newPost);
        public Task UpdateAsync(string id, ReturnPostDto updatedPost);
        public Task RemoveAsync(string id);
    }
}
