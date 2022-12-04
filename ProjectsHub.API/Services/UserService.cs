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

        internal void ChangeUserBio(Guid userId, string bio, UserRepository userRepository)
        {
            userRepository.setUserBio(userId, bio);
        }

        internal void ChangeUserName(Guid userId, UserNameDto newUserName, UserRepository userRepository)
        {
            userRepository.setUserName(userId, newUserName);
        }


        internal void ChangeUserPassword(Guid userId, PasswordUpdateDto userPasswords, UserRepository userRepository)
        {
            UserAccount user = userRepository.GetUserAccountByID(userId);
            if (user.Password.Equals(ComputePasswordHash(userPasswords.OldPassword)))
            {
                userRepository.SetUserPassword(userId, ComputePasswordHash(userPasswords.NewPassword));
                return;
            }
            throw new UserPasswordNotMatchedException();
        }

        internal void AddContact(Guid userId, Guid contactId, UserRepository userRepository)
        {
            userRepository.AddContact(userId, contactId);
        }

        internal IEnumerable<Guid> GetUserContacts(Guid userId, UserRepository userRepository)
        {
            return userRepository.GetUserContacts(userId);
        }

        internal void DeleteContact(Guid userId, Guid ContactId, UserRepository userRepository)
        {
            userRepository.DeleteContact(userId, ContactId);
        }
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 369edee (Get UserShortProfile)

        internal UserShortProfileDto GetUserShortPeofile(Guid userId, UserRepository userRepository)
        {
            var user = userRepository.GetUserById(userId);
<<<<<<< HEAD
            var userShortProfile = new UserShortProfileDto { _id = user._Id, FirstName = user.FirstName, LastName = user.LastName, ProfilePic = user.ProfilePicture };
            return userShortProfile;
        }
=======
>>>>>>> 2f10f90 (Delete user contact)
=======
            var userShortProfile = new UserShortProfileDto { _id = user._Id, UserName = $"{user.FirstName} {user.LastName}", ChangeProfilePic = user.ProfilePicture };
            return userShortProfile;
        }
>>>>>>> 369edee (Get UserShortProfile)
    }
}
