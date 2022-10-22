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
            UsersList.Add(new UserAccount { _Id = Guid.NewGuid(), FirstName = "Othman", LastName = "Othman", Email = "othman@gmail.com", Password = "123", ProfilePicture = ""});
            UsersList.Add(new UserAccount { _Id = Guid.NewGuid(), FirstName = "Noor", LastName = "Braik", Email = "noor@gmail.com", Password = "finish the API", ProfilePicture =""});
            UsersList.Add(new UserAccount { _Id = Guid.NewGuid(), FirstName = "Tariq", LastName = "Sabri", Email = "tariq@gmail.com", Password = "linux", ProfilePicture = "" });
        }

        public Object? GetUserByEmail(String Email)
        {
            return (from userAccount in UsersList
                    where userAccount.Email == Email 
                    select userAccount).FirstOrDefault();
        }

        public Guid CreateUser(UserAccountCreate user)
        {
            var _Id = Guid.NewGuid();
            UsersList.Add(new UserAccount { _Id = _Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Password = user.Password, ProfilePicture = user.ProfilePicture });
            return _Id;
        }
    }
}
