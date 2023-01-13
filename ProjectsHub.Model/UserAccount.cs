using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectsHub.Model
{
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; } = "";
        public string ProfilePicture { get; set; } = "";
        public List<string> Posts { get; set; }
        public List<string> Projects { get; set; } = new List<string>();
        public List<string> Following { get; set; } = new List<string>();
        public List<string> Followers { get; set; } =new List<string>();
        public List<string> Contacts { get; set; } = new List<string>();
    }
}