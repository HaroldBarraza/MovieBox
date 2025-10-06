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

        // CREATE
        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        // READ - Todos
        public async Task<List<Movie>> GetMoviesAsync()
        {
            return await _context.Movies
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        // READ - Por ID (método faltante)
        public async Task<Movie?> GetMovieAsync(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        // READ - Buscar por título o descripción
        public async Task<List<Movie>> SearchMoviesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetMoviesAsync();

            return await _context.Movies
                .Where(m => m.Title.Contains(searchTerm) || 
                           m.Description.Contains(searchTerm))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        // UPDATE (corregido - solo un parámetro)
        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        // DELETE
        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return false;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}