using MovieBox.Data;
using MovieBox.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieBox.Services;

public class MovieService
{
    private readonly AppDbContext _context;
    
    public MovieService(AppDbContext context)
    {
        _context = context;
    }
    
    // Obtener todas las películas
    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        return await _context.Movies
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }
    
    // Obtener una película por ID
    public async Task<Movie?> GetMovieAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }
    
    // Crear una nueva película
    public async Task<Movie> CreateMovieAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }
    
    // Eliminar una película
    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return false;
        
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }
}