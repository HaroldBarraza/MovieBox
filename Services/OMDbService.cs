using System.Text.Json;
using MovieBox.Models;

namespace MovieBox.Services
{
    public class OMDbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OMDbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OMDB_API_KEY"] ?? 
                      Environment.GetEnvironmentVariable("OMDB_API_KEY") ?? 
                      "demo"; // key demo para pruebas
        }

        public async Task<List<MovieSuggestion>> SearchMoviesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                    return new List<MovieSuggestion>();

                var url = $"http://www.omdbapi.com/?s={Uri.EscapeDataString(searchTerm)}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    return new List<MovieSuggestion>();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OMDbSearchResult>(json);

                if (result?.Search == null || result.Response == "False")
                    return new List<MovieSuggestion>();

                return result.Search.Take(5).ToList(); // Limitar a 5 sugerencias
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching movies: {ex.Message}");
                return new List<MovieSuggestion>();
            }
        }

        public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId)
        {
            try
            {
                var url = $"http://www.omdbapi.com/?i={imdbId}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MovieDetails>(json);

                return result?.Response == "True" ? result : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting movie details: {ex.Message}");
                return null;
            }
        }
    }

    // Clases para deserializar la respuesta de OMDb
    public class OMDbSearchResult
    {
        public List<MovieSuggestion> Search { get; set; } = new();
        public string TotalResults { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }

    public class MovieSuggestion
    {
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string imdbID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
    }

    public class MovieDetails
    {
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Rated { get; set; } = string.Empty;
        public string Released { get; set; } = string.Empty;
        public string Runtime { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Writer { get; set; } = string.Empty;
        public string Actors { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Awards { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string imdbRating { get; set; } = string.Empty;
        public string imdbVotes { get; set; } = string.Empty;
        public string imdbID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }
}