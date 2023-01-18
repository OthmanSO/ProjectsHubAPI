namespace ProjectsHub.Model
{
    public class ProjectReturnDto
    {
        public string _id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CoverPicture { get; set; }
        public UserShortProfileDto Author { get; set; }
        public int UsersWhoLiked { get; set; }
        public bool IsLiked { get; set; }
        public string Abstract { get; set; }
        public string ProjectFile { get; set; }
    }
}