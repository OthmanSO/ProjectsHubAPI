namespace ProjectsHub.Model
{
    public class UserAccountCreate
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }    
        public String Email { get; set; }
        public String Password { get; set; }
        public Guid ProfilePicture { get; set; }
    }
}
