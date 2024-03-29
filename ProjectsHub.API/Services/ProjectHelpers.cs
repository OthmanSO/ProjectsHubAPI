﻿using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public static class ProjectHelpers
    {
        public static Project FromCreateProject(this Project project, CreateProjectDto createProject, string userId)
        {
            project._id = null;
            project.Title = createProject.Title;
            project.ProjectFile = createProject.ProjectFile;
            project.Abstract = createProject.Abstract;
            project.UsersWhoLiked = new List<string>();
            project.AuthorId = userId;
            project.CoverPicture = createProject.CoverPicture;
            return project;
        }

        public static ProjectReturnDto ToProjectReturnDto(this Project project, UserShortProfileDto user, string loggedInUserId) =>
            new ProjectReturnDto
            {
                _id = project._id,
                Title = project.Title,
                Abstract= project.Abstract,
                UsersWhoLiked = project.UsersWhoLiked.Count(),
                Author = user,
                CreatedDate = project.CreatedDate,
                CoverPicture= project.CoverPicture,
                ProjectFile= project.ProjectFile,
                IsLiked = project.UsersWhoLiked.Any(id => id.Equals(loggedInUserId))
            };

        public static ShortProject ToShortProject(this Project post, UserShortProfileDto user, bool isFollowed, string userLoggedInId) =>
            new ShortProject
            {
                _id = post._id,
                Title = post.Title,
                Author = user,
                IsAuthorFollowed = isFollowed,
                IsLiked = post.UsersWhoLiked.Any(user => user.Equals(userLoggedInId)),
                UsersWhoLiked = post.UsersWhoLiked.Count,
                CoverPicture = post.CoverPicture,
                CreatedDate = post.CreatedDate
            };
    }
}
