using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectsHub.Core;
using ProjectsHub.Model;


namespace ProjectsHub.Data
{
    internal class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _postCollection;
        public PostRepository(IOptions<PostDBOptions> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionURI);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _postCollection = mongoDatabase.GetCollection<Post>(options.Value.PostCollectionName);
        }

        //redo this later or maybe implement it in another microservice with a Push or Pull approach
        public async Task<List<Post>> GetAsync() =>
            await _postCollection.Find(_ => true).ToListAsync();

        public async Task<Post> GetAsync(string id) =>
            await _postCollection.Find(x => x._id.Equals(id)).FirstAsync();
        public async Task<List<Post>> GetByAuthorAsync(string id) =>
            await _postCollection.Find(_ => true).ToListAsync();

        public async Task<Post> CreateAsync(Post newPost)
        {
            await _postCollection.InsertOneAsync(newPost);
            return newPost;
        }

        public async Task UpdateAsync(string id, Post updatedBook) =>
            await _postCollection.ReplaceOneAsync(x => x._id.Equals(id), updatedBook);

        public async Task RemoveAsync(string id) =>
            await _postCollection.DeleteOneAsync(x => x._id.Equals(id));

    }
}
