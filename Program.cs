using MovieBox.Components;
using MovieBox.Services;
using MovieBox.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient para TasteDive
builder.Services.AddHttpClient<TasteDiveService>();
builder.Services.AddScoped<TasteDiveService>();

// Cargar variables desde .env
LoadEnvFile();

// Configurar base de datos - SOLO UNA VEZ
var railwayUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(railwayUrl))
{
    // PRODUCCIÓN - Usar PostgreSQL de Railway
    var connectionString = ConvertRailwayUrlToConnectionString(railwayUrl);
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // DESARROLLO LOCAL - Usar SQLite
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=moviebox.db"));
}

// SIEMPRE registrar MovieService
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

string ConvertRailwayUrlToConnectionString(string railwayUrl)
{
    var uri = new Uri(railwayUrl);
    var host = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.TrimStart('/');
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo[1];
    
    return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
}

void LoadEnvFile()
{
    var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(envFilePath))
    {
        foreach (var line in File.ReadAllLines(envFilePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }
    }
}