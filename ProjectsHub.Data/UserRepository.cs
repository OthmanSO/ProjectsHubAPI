using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectsHub.Core;
using ProjectsHub.Model;

namespace ProjectsHub.Data
{
    internal class UserRepository : IUserRepository
    {
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
    }
}
