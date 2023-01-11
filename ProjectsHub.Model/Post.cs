using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectsHub.Model
{
    public class ReturnPostDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CoverPicture { get; set; }
        public string AuthorId { get; set; }
        public List<string> UsersWhoLiked { get; set; }
        public List<Chunk> PostChunks { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
