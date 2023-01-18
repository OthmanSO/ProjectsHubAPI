namespace ProjectsHub.Model
{
    public class MongoDBOptions
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string PostCollectionName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
        public string ProjectCollectionName { get; set; } = null!;
    }
}
