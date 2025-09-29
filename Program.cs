using MovieBox.Components;
using MovieBox.Services;
using MovieBox.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar base de datos
var railwayUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");
if (!string.IsNullOrEmpty(railwayUrl))
{
    // PRODUCCIÓN - Usar PostgreSQL de Railway
    var connectionString = ConvertRailwayUrlToConnectionString(railwayUrl);
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    // Reactivar MovieService solo en producción
    builder.Services.AddScoped<MovieService>();
}
else
{
    // DESARROLLO - No usar base de datos por ahora
    // builder.Services.AddScoped<MovieService>(); // Mantener comentado localmente
}

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