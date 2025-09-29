namespace MovieBox.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // Eliminar las otras propiedades que no existen en tu base de datos
}