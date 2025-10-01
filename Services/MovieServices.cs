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
    
    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        return await _context.Movies
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }
    
    public async Task<Movie?> GetMovieAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }

public async Task<Movie> CreateMovieAsync(Movie movie)
{
    try
    {
        Console.WriteLine($"=== INTENTANDO CREAR PELÍCULA ===");
        Console.WriteLine($"Título: {movie.Title}");
        Console.WriteLine($"Descripción: {movie.Description}");
        
        _context.Movies.Add(movie);
        var result = await _context.SaveChangesAsync();
        
        Console.WriteLine($"=== PELÍCULA CREADA EXITOSAMENTE ===");
        Console.WriteLine($"ID asignado: {movie.Id}");
        Console.WriteLine($"Filas afectadas: {result}");
        
        return movie;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"=== ERROR CREANDO PELÍCULA ===");
        Console.WriteLine($"Mensaje: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
        throw;
    }
}

    //Método para actualizar película
    public async Task<Movie?> UpdateMovieAsync(int id, Movie updatedMovie)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return null;

        movie.Title = updatedMovie.Title;
        movie.Description = updatedMovie.Description;
        
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return false;
        
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return true;
    }
}