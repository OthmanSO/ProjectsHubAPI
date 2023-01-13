namespace ProjectsHub.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }    
        public DateTime CreatedDate { get; set; }
        public Chunk Commentchunk { get; set; }
    }
}