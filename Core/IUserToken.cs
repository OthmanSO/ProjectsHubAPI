namespace ProjectsHub.Core
{
    public interface IUserToken
    {
        public string CreateUserToken(Guid userId, string UserName, string Email);
        public Guid GetUserIdFromToken();
    }
}
