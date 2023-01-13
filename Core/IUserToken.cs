namespace ProjectsHub.Core
{
    public interface IUserToken
    {
        public string CreateUserToken(string userId, string UserName, string Email);
        public string GetUserIdFromToken();
    }
}
