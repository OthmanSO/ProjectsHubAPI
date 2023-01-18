﻿using Microsoft.IdentityModel.Tokens;
using ProjectsHub.Core;
using ProjectsHub.Data;
using ProjectsHub.Exceptions;
using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserService _userService;

        public ProjectService(IProjectRepository projectRepository, UserService userService)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<ProjectReturnDto> CreateProject(CreateProjectDto createProject, string userId)
        {
            var user = await _userService.GetUserShortPeofile(userId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var project = new Project().FromCreateProject(createProject, userId);
            project.CreatedDate = DateTime.UtcNow;
            project = await _projectRepository.CreateAsync(project);
            
            await _userService.AddProject(userId ,project._id);

            Console.WriteLine($"user {userId} created project {project._id}");

            return project.ToProjectReturnDto(user, userId);
        }

        public async Task DeleteProject(string ProjectId, string userId)
        {
            var project = await _projectRepository.GetAsync(ProjectId);
            if (project.AuthorId != userId)
            {
                throw new UserDoesNotHavePermissionException();
            }
            await _userService.RemoveProject(userId, ProjectId);
            await _projectRepository.RemoveAsync(ProjectId);

            Console.WriteLine($"user {userId} deleted his project {ProjectId}");
        }

        public async Task<ProjectReturnDto> GetProject(string userId, string projectId)
        {
            var user = await _userService.GetUserShortPeofile(userId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var project = await _projectRepository.GetAsync(projectId);

            return project.ToProjectReturnDto(user, userId);
        }

        public async Task<ShortProject> GetShortProject(string userId, string projectId)
        {
            var project = await _projectRepository.GetAsync(projectId);
            if (project == null)
                throw new Exception(nameof(project));
            var user = await _userService.GetUserShortPeofile(userId);
            var isFollowed = await _userService.IsFollowing(userId, project.AuthorId);

            var shortPost = project.ToShortProject(user, isFollowed, userId);
            return shortPost;
        }

        public async Task<List<ShortProject>> GetUserProjectList(string loggedInUser, string userWantedProjects)
        {
            var projectsIds = await _userService.GetUserProjects(userWantedProjects);

            var isFollowed = await _userService.IsFollowing(loggedInUser, userWantedProjects);
            var user = await _userService.GetUserShortPeofile(userWantedProjects);

            var projectsListTasks = projectsIds.Select(p => _projectRepository.GetAsync(p));

            var projectsList = await Task.WhenAll(projectsListTasks);


            var shortProjectsList = projectsList.Select(p => p.ToShortProject(user, isFollowed,  userWantedProjects)).ToList();

            return shortProjectsList;
        }

        public async Task LikeProject(string userId, string projectId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
                throw new Exception();

            var project = await _projectRepository.GetAsync(projectId);
            if (project == null) throw new Exception();

            if (project.UsersWhoLiked.IsNullOrEmpty())
            {
                project.UsersWhoLiked = new List<string>();
            }
            if (!project.UsersWhoLiked.Any(x => x.Equals(userId)))
            {
                project.UsersWhoLiked.Add(userId);

                Console.WriteLine($"user {userId} now likes project {projectId}");
            }

            await _projectRepository.UpdateAsync(projectId, project);
        }

        public async Task UnLikeProject(string userId, string projectId)
        {
            var user = await _userService.GetUserProfileById(userId);
            if (user == null)
                throw new Exception();

            var post = await _projectRepository.GetAsync(projectId);
            if (post == null) throw new Exception();

            if (post.UsersWhoLiked.IsNullOrEmpty() || !post.UsersWhoLiked.Any(x => x.Equals(userId)))
            {
                return;
            }
            post.UsersWhoLiked.Remove(userId);

            Console.WriteLine($"user {userId} unliked project {projectId}");

            await _projectRepository.UpdateAsync(projectId, post);
        }
    }
}
