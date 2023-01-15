namespace ProjectsHub.Model
{
    public class PostReturnDto
    {
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CoverPicture { get; set; }
        public string AuthorId { get; set; }
        public int UsersWhoLiked { get; set; }
        public List<Chunk> PostChunks { get; set; }
        public int Comments { get; set; }
        public bool IsLiked { get; set; }
    }
}
