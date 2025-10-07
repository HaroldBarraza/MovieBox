using Microsoft.EntityFrameworkCore;
using MovieBox.Models;

namespace MovieBox.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    
    public DbSet<User> Users { get; set; }
}