namespace MovieBox.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
    }
}