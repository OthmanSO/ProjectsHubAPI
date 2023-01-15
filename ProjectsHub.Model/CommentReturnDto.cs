namespace ProjectsHub.Model
{
    public class CommentReturnDto
    {
        public int Id { get; set; }
        public UserShortProfileDto User { get; set; }
        public DateTime CreatedDate { get; set; }
        public Chunk Commentchunk { get; set; }
    }
}
