using MovieBox.Services;
using MovieBox.Data;
using MovieBox.Models;
using MovieBox.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add authentication services for Blazor Web App
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();

// Cargar variables desde .env
LoadEnvFile();

// Configurar base de datos
var railwayUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(railwayUrl))
{
    try
    {
        var connectionString = ConvertRailwayUrlToConnectionString(railwayUrl);
        Console.WriteLine($"✅ Conectando a PostgreSQL: {connectionString.Replace("Password=" + GetPasswordFromUrl(railwayUrl), "Password=***")}");
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar con PostgreSQL: {ex.Message}");
        Console.WriteLine("🔄 Usando SQLite como fallback...");
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=moviebox.db"));
    }
}
else
{
    Console.WriteLine("🔧 Usando SQLite local...");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=moviebox.db"));
}

// Registrar servicios
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<UserService>();

// Add HttpClient para OMDb
builder.Services.AddHttpClient<OMDbService>();
builder.Services.AddScoped<OMDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

// 🔥 CAMBIO IMPORTANTE: Usar el componente raíz correcto
app.MapRazorComponents<MovieBox.Components.App>()
    .AddInteractiveServerRenderMode();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

string ConvertRailwayUrlToConnectionString(string railwayUrl)
{
    railwayUrl = railwayUrl.Trim();
    
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
            database = "railway";
        }
        
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
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