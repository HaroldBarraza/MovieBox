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
        Console.WriteLine("Registering user:");
        Console.WriteLine(model.Username);
        Console.WriteLine(model.Email);
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            throw new Exception("Email already exists.");
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

    public async Task<User> SignInAsync(SigninModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            throw new Exception("Invalid email or password.");
        Console.WriteLine("User signed in:");
        Console.WriteLine(user.Username);
        Console.WriteLine(user.Email);
        Console.WriteLine(user.Id);
        Console.WriteLine(user.Role);
        return user;
    }

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
