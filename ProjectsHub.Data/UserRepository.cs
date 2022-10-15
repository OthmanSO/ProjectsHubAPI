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
            UsersList.Add(new UserAccount { UserName = "Othman Othman", Email = "Othman@gmail.com", Password = "123" });
            UsersList.Add(new UserAccount { UserName = "Noor Braik", Email = "Noor@gmail.com", Password = "finish the API" });
            UsersList.Add(new UserAccount { UserName = "Tariq Sabri", Email = "Tariq@gmail.com", Password = "linux" });
        }

        public Object? GetUser(String Email, String Password)
        {
            return (from userAccount in UsersList
                    where userAccount.Email == Email && userAccount.Password == Password
                    select userAccount).FirstOrDefault();
        }
    }
}
