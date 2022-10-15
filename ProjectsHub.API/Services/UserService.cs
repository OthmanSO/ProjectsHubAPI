using ProjectsHub.Data;
using ProjectsHub.Model;

namespace ProjectsHub.API.Services
{
    public class UserService
    {
        public Boolean UserExist(string Email, string Passwrd, UserRepository Users)
        {
            if (GetUser(Email, Passwrd, Users) == null)
                return false;
            return true;
        }
        public UserAccount GetUser(string Email, string Passwrd, UserRepository Users)
        {
            return (UserAccount)Users.GetUser(Email, Passwrd);
        }
    }
}
