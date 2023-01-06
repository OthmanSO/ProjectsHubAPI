using ProjectsHub.Data;
using ProjectsHub.Model;
using ProjectsHub.API.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace ProjectsHub.API.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository usrRepo)
        {
            this._userRepository = usrRepo ?? throw new ArgumentNullException(nameof(UserRepository));
        }
        public UserAccount GetLoggedInUser(string Email, string Password)
        {
            var user = GetUserByEmail(Email);
            if (ComputePasswordHash(Password).Equals(user.Password))
                return user;
            throw new UserPasswordNotMatchedException();
        }
        private UserAccount GetUserByEmail(string Email)
        {
            return (UserAccount)_userRepository.GetUserByEmail(Email.ToLower());
        }

        public Guid CreateUser(UserAccountCreate user)
        {
            var userAlreadyExist = GetUserByEmail(user.Email);
            if (userAlreadyExist != null)
            {
                throw new UserAlreadyExistException();
            }
            user.Password = ComputePasswordHash(user.Password);
            var userCreatedId = _userRepository.CreateUser(user);
            return userCreatedId;
        }
        private static String ComputePasswordHash(String Password)
        {
            var sha256 = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(Password);
            var byteHash = sha256.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        internal UserAccountProfileDto GetUserProfileById(Guid userId)
        {
            var user = _userRepository.GetUserById(userId);
            return user;
        }

        internal void ChangeProfilePic(Guid userId, string encodedProfilePic)
        {
            _userRepository.setProfilePic(userId, encodedProfilePic);
        }

        internal void ChangeUserBio(Guid userId, string bio)
        {
            _userRepository.setUserBio(userId, bio);
        }

        internal void ChangeUserName(Guid userId, UserNameDto newUserName)
        {
            _userRepository.setUserName(userId, newUserName);
        }


        internal void ChangeUserPassword(Guid userId, PasswordUpdateDto userPasswords)
        {
            UserAccount user = _userRepository.GetUserAccountByID(userId);
            if (user.Password.Equals(ComputePasswordHash(userPasswords.OldPassword)))
            {
                _userRepository.SetUserPassword(userId, ComputePasswordHash(userPasswords.NewPassword));
                return;
            }
            throw new UserPasswordNotMatchedException();
        }
        internal void AddContact(Guid userId, Guid contactId)
        {
            _userRepository.AddContact(userId, contactId);
        }

        internal IEnumerable<Guid> GetUserContacts(Guid userId)
        {
            return _userRepository.GetUserContacts(userId);
        }

        internal void DeleteContact(Guid userId, Guid ContactId)
        {
            _userRepository.DeleteContact(userId, ContactId);
        }

        internal UserShortProfileDto GetUserShortPeofile(Guid userId)
        {
            var user = _userRepository.GetUserById(userId);
            var userShortProfile = new UserShortProfileDto { _id = user._Id, FirstName = user.FirstName, LastName = user.LastName, ProfilePic = user.ProfilePicture };
            return userShortProfile;
        }

        internal void FollowUser(Guid userId, Guid followUserId)
        {
            //check if exist
            var loggedinUser = _userRepository.GetUserById(userId);
            var followUser = _userRepository.GetUserById(followUserId);

            _userRepository.FollowUser(userId, followUserId); 
        }

        internal void UnfollowUser(Guid userId, Guid unfollowUserId)
        {
            //check if exist
            var loggedinUser = _userRepository.GetUserById(userId);
            var followUser = _userRepository.GetUserById(unfollowUserId);

            _userRepository.UnfollowUser(userId, unfollowUserId);
        }

        internal List<Guid> GetListOfFollwers(Guid userId)
        {
            return _userRepository.GetGetListOfFollwers(userId);
        }

        internal List<Guid> GetListOfFollwing(Guid userId)
        {
            return _userRepository.GetGetListOfFollwing(userId);
        }

        internal IEnumerable<Guid> GetUserContacts(Guid userId, UserRepository userRepository)
        {
            return userRepository.GetUserContacts(userId);
        }

        internal void DeleteContact(Guid userId, Guid ContactId, UserRepository userRepository)
        {
            userRepository.DeleteContact(userId, ContactId);
        }

        internal UserShortProfileDto GetUserShortPeofile(Guid userId, UserRepository userRepository)
        {
            var user = userRepository.GetUserById(userId);
            var userShortProfile = new UserShortProfileDto { _id = user._Id, UserName = $"{user.FirstName} {user.LastName}", ChangeProfilePic = user.ProfilePicture };
            return userShortProfile;
        }
    }
}
