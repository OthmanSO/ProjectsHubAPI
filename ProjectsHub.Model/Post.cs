using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectsHub.Model
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CoverPicture { get; set; }
        public Guid AuthorId { get; set; }
        public List<Guid> UsersWhoLiked { get; set; }
        public List<Chunk> PostChunks { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
