namespace ProjectsHub.Model
{
    public class UserDBOptions
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
    }
}