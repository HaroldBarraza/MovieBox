using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBox.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int? Id { get; set; }
        
        [Required]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("role")]
        public string? Role { get; set; } = "User";

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [Column("passwordhash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}