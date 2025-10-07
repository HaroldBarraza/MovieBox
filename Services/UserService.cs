using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using MovieBox.Data;
using System.Text;
using MovieBox.Models;
namespace MovieBox.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(RegisterModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            throw new Exception("Email already exists.");
        Console.WriteLine(model.Username);
        Console.WriteLine(model.Email);
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // public async Task<User> SignInAsync(string username, string password)
    // {
    //     var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    //     if (user == null || !VerifyPassword(password, user.PasswordHash))
    //         throw new Exception("Invalid username or password.");

    //     return user;
    // }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }
}
