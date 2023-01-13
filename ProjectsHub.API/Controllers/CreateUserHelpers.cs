using ProjectsHub.Model;
using System.Text.RegularExpressions;

namespace ProjectsHub.API.Controllers
{
    public static class UserAccountCreateHelpers
    {
        public static bool IsValidEmail(this UserAccountCreate user)
        {
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(user.Email);
        }

    }
}
