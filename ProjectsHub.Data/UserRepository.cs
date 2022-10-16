using System.Collections;
using System.Linq;
using ProjectsHub.Model;

namespace ProjectsHub.Data
{
    public class UserRepository
    {
        private List<UserAccount> UsersList = new List<UserAccount>();

        public void CreateList()
        {
            UsersList.Add(new UserAccount { _Id = new Guid(), FirstName = "Othman", LastName = "Othman", Email = "othman@gmail.com", Password = "123", ProfilePicture = new Guid()});
            UsersList.Add(new UserAccount { _Id = new Guid(), FirstName = "Noor", LastName = "Braik", Email = "noor@gmail.com", Password = "finish the API", ProfilePicture =new Guid()});
            UsersList.Add(new UserAccount { _Id = new Guid(), FirstName = "Tariq", LastName = "Sabri", Email = "tariq@gmail.com", Password = "linux", ProfilePicture = new Guid()});
        }

        public Object? GetUserByEmail(String Email)
        {
            return (from userAccount in UsersList
                    where userAccount.Email == Email 
                    select userAccount).FirstOrDefault();
        }
    }
}
