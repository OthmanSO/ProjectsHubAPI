namespace ProjectsHub.Model
{
    public class UserAccount
    {
        public Guid _Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String Bio { get; set; }
        public String ProfilePicture { get; set; }
        public List<Guid>? Projects { get; set; }
        public List<Guid>? Posts { get; set; }
        public List<Guid>? Following { get; set; }
        public List<Guid>? Followers { get; set; }
        public List<Guid>? Contacts { get; set; }
    }
}