// Services/TasteDiveService.cs
using System.Text.Json;

namespace MovieBox.Services;

public class TasteDiveService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "1059644-Moviebox-D03E5954";
    private const string BaseUrl = "https://tastedive.com/api/similar";

    public TasteDiveService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MovieSuggestion>> GetMovieSuggestionsAsync(string movieTitle)
    {
        try
        {
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(movieTitle)}&k={ApiKey}&info=1&type=movies&limit=5";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TasteDiveResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Similar?.Results ?? new List<MovieSuggestion>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling TasteDive API: {ex.Message}");
            return new List<MovieSuggestion>();
        }
    }
}

public class TasteDiveResponse
{
    public Similar Similar { get; set; } = new();
}

public class Similar
{
    public List<MovieSuggestion> Results { get; set; } = new();
}

public class MovieSuggestion
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string wTeaser { get; set; } = string.Empty;
    public string wUrl { get; set; } = string.Empty;
    public string yUrl { get; set; } = string.Empty;
    public string yID { get; set; } = string.Empty;
}