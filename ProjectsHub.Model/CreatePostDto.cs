namespace ProjectsHub.Model
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string? CoverPicture { get; set; }
        public List<Chunk> PostChunks { get; set; }
    }
}