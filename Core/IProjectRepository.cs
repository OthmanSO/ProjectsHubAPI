using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IProjectRepository
    {
        public Task<List<Project>> GetAsync();
        public Task<Project> GetAsync(string id);
        public Task<List<Project>> GetByAuthorAsync(string id);
        public Task<Project> CreateAsync(Project newPost);
        public Task UpdateAsync(string id, Project updatedPost);
        public Task RemoveAsync(string id);
        public Task<List<Project>> SearchAsync(string query, int pageNo);
        public Task<List<Project>> GetAsync(List<string> projectsIdsList, int pageNo);
    }
}