using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectsHub.Model
{
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String Bio { get; set; }
        public String ProfilePicture { get; set; }
        public List<string>? Projects { get; set; }
        public List<string>? Posts { get; set; }
        public List<string>? Following { get; set; }
        public List<string>? Followers { get; set; }
        public List<string>? Contacts { get; set; }
    }
}