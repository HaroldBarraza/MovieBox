using MovieBox.Components;
using MovieBox.Services;
using MovieBox.Data;
using MovieBox.Models; // 🔥 AGREGAR ESTA LÍNEA
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Cargar variables desde .env
LoadEnvFile();

// Configurar base de datos
var railwayUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(railwayUrl))
{
    try
    {
        // PRODUCCIÓN - Usar PostgreSQL de Railway
        var connectionString = ConvertRailwayUrlToConnectionString(railwayUrl);
        Console.WriteLine($"✅ Conectando a PostgreSQL: {connectionString.Replace("Password=" + GetPasswordFromUrl(railwayUrl), "Password=***")}");
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar con PostgreSQL: {ex.Message}");
        Console.WriteLine("🔄 Usando SQLite como fallback...");
        
        // Fallback a SQLite
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=moviebox.db"));
    }
}
else
{
    // DESARROLLO LOCAL - Usar SQLite
    Console.WriteLine("🔧 Usando SQLite local...");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=moviebox.db"));
}

// Registrar servicios
builder.Services.AddScoped<MovieService>();

// Add HttpClient para OMDb
builder.Services.AddHttpClient<OMDbService>();
builder.Services.AddScoped<OMDbService>();

var app = builder.Build();

// 🔥 MINIMAL APIs
app.MapGet("/api/movies", async (MovieService movieService) =>
{
    try
    {
        var movies = await movieService.GetMoviesAsync();
        return Results.Ok(movies);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapGet("/api/movies/{id}", async (int id, MovieService movieService) =>
{
    try
    {
        var movie = await movieService.GetMovieAsync(id);
        return movie is not null ? Results.Ok(movie) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapPost("/api/movies", async (Movie movie, MovieService movieService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(movie.Title))
            return Results.BadRequest("Title is required");

        var createdMovie = await movieService.CreateMovieAsync(movie);
        return Results.Created($"/api/movies/{createdMovie.Id}", createdMovie);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapPut("/api/movies/{id}", async (int id, Movie movie, MovieService movieService) =>
{
    try
    {
        if (id != movie.Id)
            return Results.BadRequest("ID mismatch");

        var existing = await movieService.GetMovieAsync(id);
        if (existing is null)
            return Results.NotFound();

        await movieService.UpdateMovieAsync(movie);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapDelete("/api/movies/{id}", async (int id, MovieService movieService) =>
{
    try
    {
        var existing = await movieService.GetMovieAsync(id);
        if (existing is null)
            return Results.NotFound();

        var success = await movieService.DeleteMovieAsync(id);
        return success ? Results.NoContent() : Results.Problem("Delete failed");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapGet("/api/movies/search/{term}", async (string term, MovieService movieService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(term))
            return Results.BadRequest("Search term is required");

        var movies = await movieService.SearchMoviesAsync(term);
        return Results.Ok(movies);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

string ConvertRailwayUrlToConnectionString(string railwayUrl)
{
    // Limpiar y verificar la URL
    railwayUrl = railwayUrl.Trim();
    
    // Si la URL no empieza con postgresql://, agregarlo
    if (!railwayUrl.StartsWith("postgresql://") && !railwayUrl.StartsWith("postgres://"))
    {
        railwayUrl = "postgresql://" + railwayUrl;
    }
    
    Console.WriteLine($"🔗 Procesando URL: {railwayUrl.Replace(GetPasswordFromUrl(railwayUrl), "***")}");
    
    try
    {
        var uri = new Uri(railwayUrl);
        var host = uri.Host;
        var port = uri.Port;
        var database = uri.AbsolutePath.TrimStart('/');
        
        if (string.IsNullOrEmpty(database))
        {
            database = "railway"; // valor por defecto
        }
        
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        
        var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        
        return connectionString;
    }
    catch (UriFormatException ex)
    {
        throw new Exception($"Formato de URL inválido: {railwayUrl}. Error: {ex.Message}");
    }
}

string GetPasswordFromUrl(string url)
{
    try
    {
        var uri = new Uri(url.StartsWith("postgres") ? url : "postgresql://" + url);
        var userInfo = uri.UserInfo.Split(':');
        return userInfo.Length > 1 ? userInfo[1] : "";
    }
    catch
    {
        return "";
    }
}

void LoadEnvFile()
{
    var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(envFilePath))
    {
        Console.WriteLine($"📁 Cargando variables desde: {envFilePath}");
        foreach (var line in File.ReadAllLines(envFilePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                Environment.SetEnvironmentVariable(key, value);
                Console.WriteLine($"   {key} = { (key.ToUpper().Contains("PASSWORD") || key.ToUpper().Contains("SECRET") ? "***" : value) }");
            }
        }
    }
    else
    {
        Console.WriteLine($"ℹ️ No se encontró archivo .env en: {envFilePath}");
    }
    
    // Verificar si DATABASE_PUBLIC_URL está cargado
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");
    if (string.IsNullOrEmpty(dbUrl))
    {
        Console.WriteLine("⚠️ DATABASE_PUBLIC_URL no encontrado en variables de entorno");
    }
    else
    {
        Console.WriteLine($"✅ DATABASE_PUBLIC_URL cargado: {dbUrl.Replace(GetPasswordFromUrl(dbUrl), "***")}");
    }
}