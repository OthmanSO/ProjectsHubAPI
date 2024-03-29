﻿using ProjectsHub.Core;
using MongoDB.Driver;
using ProjectsHub.Model;
using Microsoft.Extensions.Options;


namespace ProjectsHub.Data
{
    internal class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<Project> _projectCollection;
        private static readonly int PAGESIZE = 20;
        public ProjectRepository(IOptions<MongoDBOptions> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionURI);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _projectCollection = mongoDatabase.GetCollection<Project>(options.Value.ProjectCollectionName);
        }

        //redo this later or maybe implement it in another microservice with a Push or Pull approach
        public async Task<List<Project>> GetAsync() =>
            await _projectCollection.Find(_ => true).ToListAsync();

        public async Task<Project> GetAsync(string id) =>
            await _projectCollection.Find(x => x._id.Equals(id)).FirstAsync();
        public async Task<List<Project>> GetByAuthorAsync(string id) =>
            await _projectCollection.Find(x => x.AuthorId.Equals(id)).ToListAsync();

        public async Task<Project> CreateAsync(Project newProject)
        {
            await _projectCollection.InsertOneAsync(newProject);
            return newProject;
        }

        public async Task UpdateAsync(string id, Project updatedProject) =>
            await _projectCollection.ReplaceOneAsync(x => x._id.Equals(id), updatedProject);

        public async Task RemoveAsync(string id) =>
            await _projectCollection.DeleteOneAsync(x => x._id.Equals(id));


        public async Task<List<Project>> GetAsync(List<string> projectIdsList, int pageNo) =>
            _projectCollection.AsQueryable()
            .Where(project => projectIdsList.Contains(project._id))
            .Select(project => project)
            .OrderBy(project => project.CreatedDate)
            .Skip(PAGESIZE * (pageNo - 1))
            .Take(PAGESIZE)
            .ToList();

        public async Task<List<Project>> SearchAsync(string query, int pageNo) =>
            _projectCollection.AsQueryable()
            .Where(project =>
                project.Title.Contains(query)
                || project.Abstract.Contains(query)
            )
            .Select(project => project)
            .OrderBy(project => project.CreatedDate)
            .Skip(PAGESIZE * (pageNo - 1))
            .Take(PAGESIZE)
            .ToList();
    }
}