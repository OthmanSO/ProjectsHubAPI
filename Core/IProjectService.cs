using ProjectsHub.Model;

namespace ProjectsHub.Core
{
    public interface IProjectService
    {
        public Task<ProjectReturnDto> CreateProject(CreateProjectDto Project, string userId);
        public Task<ProjectReturnDto> GetProject(string userid, string projectId);
        public Task DeleteProject(string ProjectId, string userId);
        public Task LikeProject(string userId, string projectId);
        public Task UnLikeProject(string userId, string projectId);
        public Task<ShortProject> GetShortProject(string userId, string projectId);
        public Task<List<ShortProject>> GetUserProjectList(string loggedInUser, string userWantedProjects);
    }
}
