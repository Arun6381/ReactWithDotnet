namespace curdinStoredprocedure.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string? Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
