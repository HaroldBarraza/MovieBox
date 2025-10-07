namespace MovieBox.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string PasswordHash { get; set; } = string.Empty;
}