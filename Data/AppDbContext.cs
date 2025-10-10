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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; } // <- Esta línea debe estar presente

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración para Movies
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("movies");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Plot).HasColumnName("plot");
                entity.Property(e => e.Poster).HasColumnName("poster");
                entity.Property(e => e.Genre).HasColumnName("genre");
            });

            // Configuración para Comments
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserName).HasColumnName("username").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Text).HasColumnName("text").IsRequired().HasMaxLength(500);
                entity.Property(e => e.Rating).HasColumnName("rating").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("createdat").IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.HelpfulVotes).HasColumnName("helpfulvotes").IsRequired().HasDefaultValue(0);
                entity.Property(e => e.TotalVotes).HasColumnName("totalvotes").IsRequired().HasDefaultValue(0);
                entity.Property(e => e.MovieId).HasColumnName("movieid").IsRequired();

                // Relación con Movie
                entity.HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey(c => c.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 🔥 AGREGAR CONFIGURACIÓN PARA USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("passwordhash").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasDefaultValue("User");
            });
        }
    }
}