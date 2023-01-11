
using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IUserRepository
    {
        public Task<List<UserAccount>> GetAsync();
        public Task<UserAccount> GetAsync(string id);
        public Task<List<UserAccount>> GetByEmailAsync(string id);
        public Task<UserAccount> CreateAsync(string Email);
        public Task UpdateAsync(string id, UserAccount updatedUserAccount);
        public Task RemoveAsync(string id);
    }
}