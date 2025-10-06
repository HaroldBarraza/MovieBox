using Microsoft.EntityFrameworkCore;
using MovieBox.Models;

namespace MovieBox.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("movies");
                
                entity.HasKey(e => e.Id);
                
                // 🔥 USAR PASCALCASE (Id, Title, Description, etc.)
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Plot).HasColumnName("plot");
                entity.Property(e => e.Poster).HasColumnName("poster");
                entity.Property(e => e.Genre).HasColumnName("genre");
            });
        }
    }
}