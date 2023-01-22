using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectsHub.Core;
using ProjectsHub.Model;

namespace ProjectsHub.Data
{
    internal class UserRepository : IUserRepository
    {
        private static readonly int PAGESIZE = 20;
        private readonly IMongoCollection<UserAccount> _userCollection;
        public UserRepository(IOptions<MongoDBOptions> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionURI);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<UserAccount>(options.Value.UserCollectionName);
        }

        public async Task<UserAccount> CreateAsync(UserAccount userAccount)
        {
            await _userCollection.InsertOneAsync(userAccount);
            return userAccount;
        }

        public async Task UpdateAsync(string id, UserAccount updatedUserAccount) =>
           await _userCollection.ReplaceOneAsync(user => user._Id.Equals(id), updatedUserAccount);

        public async Task<UserAccount> GetAsync(string id) =>
          await _userCollection.Find(user => user._Id.Equals(id)).FirstAsync();


        public async Task<UserAccount> GetByEmailAsync(string Email) =>
           await _userCollection.Find(user => user.Email.Equals(Email)).FirstOrDefaultAsync();


        public async Task RemoveAsync(string id) =>
           await _userCollection.DeleteOneAsync(user => user._Id.Equals(id));

        public async Task<List<UserAccount>> GetAsync(List<string> listOfFollowing, int pageNo, string loggedInUser)
        {
            return _userCollection.AsQueryable()
                .Where(user => !listOfFollowing.Contains(user._Id))
                .Where(user => !user._Id.Equals(loggedInUser))
                .Select(user => user)
                .Skip(PAGESIZE * (pageNo - 1))
                .Take(PAGESIZE)
                .ToList();
        }

        public async Task<List<UserAccount>> SearchAsync(string query, int pageNo, string loggedInUserId) =>
             _userCollection
            .AsQueryable()
            .Where(user => (
            user.FirstName.ToLower().Contains(query.ToLower()) 
            || user.LastName.ToLower().Contains(query.ToLower())))
            .Where(user => !user._Id.Equals(loggedInUserId))
            .Select(user => user)
            .Skip(PAGESIZE * (pageNo - 1))
            .Take(PAGESIZE)
            .ToList();
    }
}
