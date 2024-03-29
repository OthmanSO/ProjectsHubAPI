﻿using ProjectsHub.Core;
using ProjectsHub.Model;
using ProjectsHub.Exceptions;
using ProjectsHub.API.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace ProjectsHub.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository usrRepo)
        {
            this._userRepository = usrRepo ?? throw new ArgumentNullException(nameof(IUserRepository));
        }
        public async Task<UserAccount> GetLoggedInUser(string Email, string Password)
        {
            var user = await _userRepository.GetByEmailAsync(Email.ToLower());
            var EncodedPassword = Password.ComputePasswordHash();
            if (EncodedPassword.Equals(user.Password))
                return user;
            throw new UserPasswordNotMatchedException();
        }

        public async Task<UserAccount> CreateUser(UserAccountCreate user)
        {
            if (!user.IsValidEmail())
            {
                throw new BadEmailException();
            }
            var userAlreadyExist = await _userRepository.GetByEmailAsync(user.Email.ToLower());
            if (userAlreadyExist != null)
            {
                throw new UserAlreadyExistException();
            }

            var createUser = new UserAccount();
            createUser.FromUserAccountCreateDto(user);

            var createdUser = await _userRepository.CreateAsync(createUser);
            return createdUser;
        }

        public async Task<UserAccountProfileDto> GetUserProfileById(string userId) =>
            (await _userRepository.GetAsync(userId)).ToUserAccountProfileDto();

        public async Task ChangeProfilePic(string userId, string encodedProfilePic)
        {
            var user = await _userRepository.GetAsync(userId);
            user.ProfilePicture = encodedProfilePic;
            await _userRepository.UpdateAsync(userId, user);
        }

        public async Task ChangeUserBio(string userId, string bio)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Bio = bio;
            await _userRepository.UpdateAsync(userId, user);
        }

        public async Task ChangeUserName(string userId, UserNameDto newUserName)
        {
            var user = await _userRepository.GetAsync(userId);
            user.FirstName = newUserName.FirstName;
            user.LastName = newUserName.LastName;
            await _userRepository.UpdateAsync(userId, user);
        }


        public async Task ChangeUserPassword(string userId, PasswordUpdateDto userPasswords)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user.Password.Equals(userPasswords.OldPassword.ComputePasswordHash()))
            {
                user.Password = userPasswords.NewPassword.ComputePasswordHash();
                await _userRepository.UpdateAsync(userId, user);
                return;
            }
            throw new UserPasswordNotMatchedException();
        }
        public async Task AddContact(string userId, string contactId)
        {
            var user1 = await _userRepository.GetAsync(userId);
            var user2 = await _userRepository.GetAsync(contactId);

            if (user1.Contacts.IsNullOrEmpty() || !user1.Contacts.Any(x => x.Equals(contactId)))
            {
                user1.Contacts.Add(contactId);
            }

            if (user2.Contacts.IsNullOrEmpty() || !user2.Contacts.Any(x => x.Equals(userId)))
            {
                user2.Contacts.Add(userId);
            }
            await _userRepository.UpdateAsync(userId, user1);
            await _userRepository.UpdateAsync(contactId, user2);
        }

        public async Task<List<string>> GetUserContacts(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            return user.Contacts;
        }

        public async Task DeleteContact(string userId, string contactId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Contacts.Remove(contactId);
            await _userRepository.UpdateAsync(userId, user);
        }

        public async Task<UserShortProfileDto> GetUserShortPeofile(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            var userShortProfile = user.ToUserShortProfileDto();
            return userShortProfile;
        }

        public async Task FollowUser(string userId, string followUserId)
        {
            //check if exist
            var user = await _userRepository.GetAsync(userId);
            var followedUser = await _userRepository.GetAsync(followUserId);

            if (user.Following.IsNullOrEmpty() || !user.Following.Any(x => x.Equals(followUserId)))
            {
                user.Following.Add(followUserId);
            }
            if (followedUser.Followers.IsNullOrEmpty() || !followedUser.Followers.Any(x => x.Equals(userId)))
            {
                followedUser.Followers.Add(userId);
            }
            await _userRepository.UpdateAsync(userId, user);
            await _userRepository.UpdateAsync(followUserId, followedUser);
        }

        public async Task UnfollowUser(string userId, string unfollowUserId)
        {
            //check if exist
            var user = await _userRepository.GetAsync(userId);
            var unfollowedUser = await _userRepository.GetAsync(unfollowUserId);

            user.Following.Remove(unfollowUserId);
            unfollowedUser.Followers.Remove(userId);

            await _userRepository.UpdateAsync(userId, user);
            await _userRepository.UpdateAsync(unfollowUserId, unfollowedUser);
        }

        public async Task<List<string>> GetListOfFollwers(string userId) =>
            (await _userRepository.GetAsync(userId)).Followers;


        public async Task<List<string>> GetListOfFollwing(string userId) =>
            (await _userRepository.GetAsync(userId)).Following;

        internal async Task AddPost(string userId, string postId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Posts.Insert(0, postId);
            await _userRepository.UpdateAsync(userId, user);
        }
        internal async Task AddProject(string userId, string projectId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Projects.Insert(0, projectId);
            await _userRepository.UpdateAsync(userId, user);
        }

        internal async Task RemovePost(string userId, string postId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Posts.Remove(postId);
            await _userRepository.UpdateAsync(userId, user);
        }

        internal async Task<bool> IsFollowing(string userId, string authorId)
        {
            var listOfFollowing = await GetListOfFollwing(userId);
            if (listOfFollowing == null)
                return false;
            return listOfFollowing.Any(x => x.Equals(authorId));
        }

        internal async Task<List<string>> GetUserPosts(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user.Posts == null)
                return new List<string>();

            return user.Posts.ToList();
        }

        internal async Task RemoveProject(string userId, string projectId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.Projects.Remove(projectId);
            await _userRepository.UpdateAsync(userId, user);
        }

        internal async Task<List<string>> GetUserProjects(string userWantedProjects)
        {
            var user = await _userRepository.GetAsync(userWantedProjects);
            if (user.Projects == null)
                return new List<string>();

            return user.Projects.ToList();
        }

        internal async Task<List<UserNetworkProfile>> GetUserNetwork(string id, int pageNo)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                throw new Exception();

            try
            {
                var users = await _userRepository.GetAsync(user.Following, pageNo, id);

                var userNetworkList = new List<UserNetworkProfile>();
                foreach (var usr in users)
                {
                    userNetworkList.Add(usr.ToUserNetworkProfile(id));
                }
                return userNetworkList;
            }
            catch (Exception)
            {
                return new List<UserNetworkProfile>();
            }
        }

        internal async Task<List<UserNetworkProfile>> SearchUserNetwork(string query, string id, int pageNo)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                throw new Exception();

            try
            {
                var users = await _userRepository.SearchAsync(query, pageNo, id);

                var userNetworkList = new List<UserNetworkProfile>();
                foreach (var usr in users)
                {
                    userNetworkList.Add(usr.ToUserNetworkProfile(id));
                }
                return userNetworkList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return new List<UserNetworkProfile>();
            }
        }
    }
}
