using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectsHub.Model
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CoverPicture { get; set; }
        public string AuthorId { get; set; }
        public List<string> UsersWhoLiked { get; set; }
        public string Abstract { get; set; }
        public string ProjectFile { get; set; }
    }
}