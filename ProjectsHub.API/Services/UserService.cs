using ProjectsHub.Data;
using ProjectsHub.Model;
using ProjectsHub.API.Exceptions;

namespace ProjectsHub.API.Services
{
    public class UserService
    {
        public UserAccount GetLoggedInUser(string Email, string Password, UserRepository Users) 
        {
            var user = GetUserByEmail(Email, Users);
            if (user.Password == Password)
                return user;
            throw new UserPasswordNotMatchedException();
        }
        private UserAccount GetUserByEmail(string Email, UserRepository Users)
        {
            return (UserAccount)Users.GetUserByEmail(Email.ToLower());
        }
    }
}
