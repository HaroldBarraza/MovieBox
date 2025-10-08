namespace MovieBox.Authentication
{
    public class UserSession
    {
        public string? Username { get; set; }
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}