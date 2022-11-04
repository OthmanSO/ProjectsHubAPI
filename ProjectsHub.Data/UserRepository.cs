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

        public void CreateList()
        {
            UsersList.Add(new UserAccount
            {
                _Id = Guid.NewGuid(),
                FirstName = "Othman",
                LastName = "Othman",
                Email = "othman@gmail.com",
                Password = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3",
                ProfilePicture = "",
                Posts = new List<Guid>(),
                Projects = new List<Guid>(),
                Bio = "",
                Followers = new List<Guid>(),
                Following = new List<Guid>()
            });
            UsersList.Add(new UserAccount
            {
                _Id = Guid.NewGuid(),
                FirstName = "Noor",
                LastName = "Braik",
                Email = "noor@gmail.com",
                Password = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3",
                ProfilePicture = "",
                Posts = new List<Guid>(),
                Projects = new List<Guid>(),
                Bio = "",
                Followers = new List<Guid>(),
                Following = new List<Guid>()
            });
            UsersList.Add(new UserAccount
            {
                _Id = Guid.NewGuid(),
                FirstName = "Tariq",
                LastName = "Sabri",
                Email = "tariq@gmail.com",
                Password = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3",
                ProfilePicture = "",
                Posts = new List<Guid>(),
                Projects = new List<Guid>(),
                Bio = "",
                Followers = new List<Guid>(),
                Following = new List<Guid>()
            });
        }

        public UserAccount? GetUserByEmail(String Email)
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
                        select userAccount).FirstOrDefault();

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
                Name = $"{ user.FirstName} {user.LastName}",
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                Following = user.Following != null ? user.Following.Count() : 0,
                Followers = user.Followers != null ? user.Followers.Count() : 0,
                Posts = lastFivePosts,
                Projects = lastFiveProjects
            };
        }
    }
}
