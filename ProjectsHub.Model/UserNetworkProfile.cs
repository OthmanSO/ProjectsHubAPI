namespace ProjectsHub.Model
{
    public class UserNetworkProfile
    {
        public string _id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public bool IsFollowed { get; set; }
        public string Bio { get; set; }
    }
}