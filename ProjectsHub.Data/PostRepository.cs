using ProjectsHub.Core;
using MongoDB.Driver;
using ProjectsHub.Model;

namespace ProjectsHub.Data
{
    internal class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _postCollection;
        internal PostRepository(PostDBOptions options)
        {
            var mongoClient = new MongoClient(options.ConnectionURI);

            var mongoDatabase = mongoClient.GetDatabase(options.DatabaseName);

            _postCollection = mongoDatabase.GetCollection<Post>(options.PostCollectionName);
        }
        public async Task<List<Post>> GetAsync() =>
            await _postCollection.Find(_ => true).ToListAsync();

        public async Task<Post> GetAsync(string id) =>
            await _postCollection.Find(x => x._id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Post newBook) =>
            await _postCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Post updatedBook) =>
            await _postCollection.ReplaceOneAsync(x => x._id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _postCollection.DeleteOneAsync(x => x._id == id);

        public async Task<List<Post>> GetByAuthorAsync(string id) =>
            await _postCollection.Find(_ => true).ToListAsync();
    }
}
