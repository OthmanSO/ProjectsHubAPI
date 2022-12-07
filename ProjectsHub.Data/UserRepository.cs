using System.Collections;
using System.Linq;
using System.Text;
using ProjectsHub.Model;
using System.Security.Cryptography;

namespace ProjectsHub.Data
{
    public class UserRepository
    {
        private List<UserAccount> UsersList = new List<UserAccount>();

        public void setProfilePic(Guid userId, string encodedProfilePic)
        {
            var userAccount = GetUserAccountByID(userId);   
            userAccount.ProfilePicture = encodedProfilePic;
        }

        public void setUserName(Guid userId, UserNameDto newUserName)
        {
            var userAccount = GetUserAccountByID(userId);
            userAccount.FirstName = newUserName.FirstName;
            userAccount.LastName = newUserName.LastName;
        }

        public void SetUserPassword(Guid userId, string password)
        {
            var userAccount = GetUserAccountByID(userId);
            userAccount.Password = password;
        }

        public UserAccount GetUserAccountByID(Guid userId)
        {
            return UsersList.First(x => x._Id == userId);
        }

        public IEnumerable<Guid> GetUserContacts(Guid userId)
        {
            var user = GetUserAccountByID(userId);
            return user.Contacts != null ? user.Contacts : new List<Guid>();
        }

        public void DeleteContact(Guid userId, Guid contactId)
        {
            var usr = GetUserAccountByID(userId);
            usr.Contacts.Remove(contactId);
        }

        public void AddContact(Guid userId, Guid contactId)
        {
            var user1 = GetUserAccountByID(userId); 
            var user2 = GetUserAccountByID(contactId);

            if (user1.Contacts == null)
                user1.Contacts = new List<Guid>();

            if (user2.Contacts == null)
                user2.Contacts = new List<Guid>();

            if (!user1.Contacts.Any(x => x.Equals(contactId)))
            {
                user1.Contacts.Add(contactId);
            }

            if (!user2.Contacts.Any(x => x.Equals(userId)))
            {
                user2.Contacts.Add(userId);
            }
        }

        public void setUserBio(Guid userId, String Bio)
        {
            var userAccount = GetUserAccountByID(userId);
            userAccount.Bio = Bio;
        }

        public UserAccount GetUserByEmail(String Email)
        {
            return (from userAccount in UsersList
                    where userAccount.Email == Email
                    select userAccount).FirstOrDefault();
        }

        public Guid CreateUser(UserAccountCreate user)
        {
            var _Id = Guid.NewGuid();
            UsersList.Add(new UserAccount { _Id = _Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Password = user.Password, ProfilePicture = user.ProfilePicture });
            Console.WriteLine(_Id);
            return _Id;
        }

        public UserAccountProfileDto GetUserById(Guid userId)
        {
            List<Guid> lastFivePosts = new List<Guid>();
            List<Guid> lastFiveProjects = new List<Guid>();

            var user = (from userAccount in UsersList
                        where userAccount._Id == userId
                        select userAccount).First();

            if (user == null)
            {
                Console.WriteLine("no user");
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Posts != null)
            {

                lastFivePosts = (from post in user.Posts
                                 select post).Take(5).ToList();
            }

            if (user.Projects != null)
            {
                lastFiveProjects = (from post in user.Projects
                                    select post).Take(5).ToList();
            }

            return new UserAccountProfileDto
            {
                _Id = user._Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                Following = user.Following != null ? user.Following.Count() : 0,
                Followers = user.Followers != null ? user.Followers.Count() : 0,
                Posts = lastFivePosts,
                Projects = lastFiveProjects
            };
        }

        public void FollowUser(Guid userId, Guid followUserId)
        {
            var user = (from userAccount in UsersList
                        where userId == userAccount._Id
                        select userAccount).First();
            user.Following.Add(followUserId);
        }
    }
}
