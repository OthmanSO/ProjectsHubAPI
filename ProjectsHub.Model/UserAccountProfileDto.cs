namespace ProjectsHub.Model
{
    public class UserAccountProfileDto
    {
        public string _Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String ProfilePicture { get; set; }
        public String Bio { get; set; }
        public int Following { get; set; }
        public int Followers { get; set; }
        public IEnumerable<string>? Projects { get; set; }
        public IEnumerable<string>? Posts { get; set; }
    }
}
