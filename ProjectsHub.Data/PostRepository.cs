﻿using ProjectsHub.Core;
using MongoDB.Driver;
using ProjectsHub.Model;
using Microsoft.Extensions.Options;


namespace ProjectsHub.Data
{
    internal class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _postCollection;
        private readonly static int PAGESIZE = 20;

        public PostRepository(IOptions<MongoDBOptions> options)
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
            await _postCollection.Find(x => x.AuthorId.Equals(id)).ToListAsync();

        public async Task<Post> CreateAsync(Post newPost)
        {
            await _postCollection.InsertOneAsync(newPost);
            return newPost;
        }

        public async Task UpdateAsync(string id, Post updatedPost) =>
            await _postCollection.ReplaceOneAsync(x => x._id.Equals(id), updatedPost);

        public async Task RemoveAsync(string id) =>
            await _postCollection.DeleteOneAsync(x => x._id.Equals(id));

        public async Task<List<Post>> GetAsync(List<string> postIdsList, int pageNo) =>
            _postCollection.AsQueryable()
            .Where(post => postIdsList.Contains(post._id))
            .Select(post => post)
            .OrderBy(post => post.CreatedDate)
            .Skip(PAGESIZE * (pageNo - 1))
            .Take(PAGESIZE)
            .ToList();

        public async Task<List<Post>> SearchAsync(string query, int pageNo) =>
            _postCollection.AsQueryable()
            .Where(post =>
                post.Title.Contains(query)
                || post.PostChunks.Any(c =>
                    c.Body.Contains(query))
            )
            .Select(post => post)
            .OrderBy(post => post.CreatedDate)
            .Skip(PAGESIZE * (pageNo - 1))
            .Take(PAGESIZE)
            .ToList();
    }
}
