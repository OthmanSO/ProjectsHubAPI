namespace ProjectsHub.Model
{
    public class UserAccountProfileDto
    {
        public Guid _Id { get; set; }
        public String Name { get; set; }
        public String ProfilePicture { get; set; }
        public String Bio { get; set; }
        public int Following { get; set; }
        public int Followers { get; set; }
        public IEnumerable<Guid>? Projects { get; set; }
        public IEnumerable<Guid>? Posts { get; set; }
    }
}
