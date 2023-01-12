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
            userAccount.Email = createUserAccount.Email;
            userAccount.Password = createUserAccount.Password.ComputePasswordHash();
            userAccount.ProfilePicture = createUserAccount.ProfilePicture;
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
                Posts = userAccount.Posts.Take(5),
                Projects = userAccount.Projects.Take(5)
            };
        }
    }
}
