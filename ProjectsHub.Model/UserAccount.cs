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
        public IEnumerable<Guid> Projects { get; set; }
        public IEnumerable<Guid> Posts { get; set; }
        public IEnumerable<Guid> Following { get; set; }
        public IEnumerable<Guid> Followers { get; set; }
        public IEnumerable<Guid> Contacts { get; set; }
    }
}