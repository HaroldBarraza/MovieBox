namespace MovieBox.Authentication
{
    public class UserSession
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
