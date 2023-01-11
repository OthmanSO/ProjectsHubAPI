namespace ProjectsHub.Model
{
    public class Comment
    {
        public Guid UserId { get; set; }
        public Chunk Commentchunk { get; set; }
        public List<Comment> Replys { get; set; }
    }
}