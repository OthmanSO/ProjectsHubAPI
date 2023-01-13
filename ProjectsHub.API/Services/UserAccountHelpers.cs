using ProjectsHub.Model;
using System.Text;
using System.Security.Cryptography;

namespace ProjectsHub.API.Services
{
    public static class UserAccountHelpers
    {
        public static void FromUserAccountCreateDto(this UserAccount userAccount, UserAccountCreate createUserAccount)
        {
            userAccount.FirstName = createUserAccount.FirstName;
            userAccount.LastName = createUserAccount.LastName;
            userAccount.Email = createUserAccount.Email.ToLower();
            userAccount.Password = createUserAccount.Password.ComputePasswordHash();
            userAccount.ProfilePicture = createUserAccount.ProfilePicture;
            userAccount.Bio = "";
            userAccount.Contacts = new List<string>();
            userAccount.Followers= new List<string>();
            userAccount.Following= new List<string>();
            userAccount.Projects= new List<string>();
            userAccount.Posts= new List<string>();
        }

        public static string ComputePasswordHash(this String Password)
        {
            var sha256 = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(Password);
            var byteHash = sha256.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        public static UserAccountProfileDto ToUserAccountProfileDto(this UserAccount userAccount)
        {
            return new UserAccountProfileDto
            {
                _Id = userAccount._Id,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                ProfilePicture = userAccount.ProfilePicture,
                Bio = userAccount.Bio,
                Followers = userAccount.Followers.Count,
                Following = userAccount.Following.Count,
                Posts = userAccount.Posts != null ? userAccount.Posts.Take(5) : new List<string>(),
                Projects = userAccount.Projects != null ? userAccount.Projects.Take(5) : new List<string>()
            };
        }
    }
}
