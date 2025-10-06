using Microsoft.EntityFrameworkCore;
using MovieBox.Data;
using MovieBox.Models;

namespace MovieBox.Services
{
    public class MovieService
    {
        private readonly AppDbContext _context;

        public MovieService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            try
            {
                return await _context.Movies
                    .FromSqlRaw("SELECT id, title, description, plot, poster, genre FROM movies ORDER BY title")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading movies: {ex.Message}");
                return new List<Movie>();
            }
        }

        public async Task<Movie?> GetMovieAsync(int id)
        {
            try
            {
                return await _context.Movies
                    .FromSqlRaw("SELECT id, title, description, plot, poster, genre FROM movies WHERE id = {0}", id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting movie: {ex.Message}");
                return null;
            }
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO movies (title, description, plot, poster, genre) VALUES ({0}, {1}, {2}, {3}, {4})",
                    movie.Title, 
                    movie.Description ?? "", 
                    movie.Plot ?? "",
                    movie.Poster ?? "",
                    movie.Genre ?? "");

                var newMovie = await _context.Movies
                    .FromSqlRaw("SELECT id, title, description, plot, poster, genre FROM movies ORDER BY id DESC LIMIT 1")
                    .FirstOrDefaultAsync();

                return newMovie ?? movie;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating movie: {ex.Message}");
                throw;
            }
        }

        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE movies SET title = {0}, description = {1}, plot = {2}, poster = {3}, genre = {4} WHERE id = {5}",
                    movie.Title, 
                    movie.Description ?? "", 
                    movie.Plot ?? "",
                    movie.Poster ?? "",
                    movie.Genre ?? "",
                    movie.Id);

                return movie;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating movie: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM movies WHERE id = {0}", id);

                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting movie: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Movie>> SearchMoviesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetMoviesAsync();

                return await _context.Movies
                    .FromSqlRaw("SELECT id, title, description, plot, poster, genre FROM movies WHERE title ILIKE {0} OR description ILIKE {0} OR plot ILIKE {0} OR genre ILIKE {0} ORDER BY title", 
                    $"%{searchTerm}%")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching movies: {ex.Message}");
                return new List<Movie>();
            }
        }
    }
}