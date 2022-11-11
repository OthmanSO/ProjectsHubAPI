using ProjectsHub.Data;
using ProjectsHub.Model;
using ProjectsHub.API.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace ProjectsHub.API.Services
{
    public class UserService
    {
        public UserAccount GetLoggedInUser(string Email, string Password, UserRepository Users)
        {
            var user = GetUserByEmail(Email, Users);
            if (ComputePasswordHash(Password).Equals(user.Password))
                return user;
            throw new UserPasswordNotMatchedException();
        }
        private UserAccount GetUserByEmail(string Email, UserRepository Users)
        {
            return (UserAccount)Users.GetUserByEmail(Email.ToLower());
        }

        public Guid CreateUser(UserAccountCreate user, UserRepository userRepository)
        {
            var userAlreadyExist = GetUserByEmail(user.Email, userRepository);
            if (userAlreadyExist != null)
            {
                throw new UserAlreadyExistException();
            }
            user.Password = ComputePasswordHash(user.Password);
            var userCreatedId = userRepository.CreateUser(user);
            return userCreatedId;
        }
        private static String ComputePasswordHash(String Password)
        {
            var sha256 = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(Password);
            var byteHash = sha256.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        internal UserAccountProfileDto GetUserProfileById(Guid userId, UserRepository userRepository)
        {
            var user = userRepository.GetUserById(userId);
            return user;
        }

        internal void ChangeProfilePic(Guid userId, string encodedProfilePic, UserRepository userRepository)
        {
                userRepository.setProfilePic(userId, encodedProfilePic);
        }
    }
}
