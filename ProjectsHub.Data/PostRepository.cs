using ProjectsHub.Core;
using MongoDB.Driver;
using ProjectsHub.Model;
using Microsoft.Extensions.Options;


namespace ProjectsHub.Data
{
    internal class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<ReturnPostDto> _postCollection;
        public PostRepository(IOptions<PostDBOptions> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionURI);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _postCollection = mongoDatabase.GetCollection<ReturnPostDto>(options.Value.PostCollectionName);
        }

        //redo this later or maybe implement it in another microservice with a Push or Pull approach
        public async Task<List<ReturnPostDto>> GetAsync() =>
            await _postCollection.Find(_ => true).ToListAsync();

        public async Task<ReturnPostDto> GetAsync(string id) =>
            await _postCollection.Find(x => x._id.Equals(id)).FirstAsync();
        public async Task<List<ReturnPostDto>> GetByAuthorAsync(string id) =>
            await _postCollection.Find(_ => true).ToListAsync();

        public async Task<ReturnPostDto> CreateAsync(ReturnPostDto newPost)
        {
            await _postCollection.InsertOneAsync(newPost);
            return newPost;
        }

        public async Task UpdateAsync(string id, ReturnPostDto updatedPost) =>
            await _postCollection.ReplaceOneAsync(x => x._id.Equals(id), updatedPost);

        public async Task RemoveAsync(string id) =>
            await _postCollection.DeleteOneAsync(x => x._id.Equals(id));

    }
}
