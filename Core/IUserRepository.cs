
using MongoDB.Bson;
using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IUserRepository
    {
        public Task<UserAccount> GetAsync(string id);
        public Task<UserAccount> GetByEmailAsync(string Email);
        public Task<UserAccount> CreateAsync(UserAccount userAccount);
        public Task UpdateAsync(string id, UserAccount updatedUserAccount);
        public Task RemoveAsync(string id);
        public Task<List<UserAccount>> GetAsync(List<string> ListOfFollowing, int PageNo);
    }
}