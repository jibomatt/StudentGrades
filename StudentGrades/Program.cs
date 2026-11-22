using Microsoft.EntityFrameworkCore;
using StudentGrades.Data;

var builder = WebApplication.CreateBuilder(args);

// Free hosts like Render tell the app which port to use through the PORT variable.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Connection string: appsettings.json locally, or the environment variable
// ConnectionStrings__DefaultConnection (or DATABASE_URL) when hosted.
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") is { Length: > 0 } url
    ? ConvertPostgresUrl(url)
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations and create/refresh the view on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    db.Database.ExecuteSqlRaw(AppDbContext.CreateViewSql);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=StudentGrades}/{action=Index}/{id?}");

app.Run();

// Converts "postgresql://user:pass@host:5432/db?sslmode=require" into Npgsql key=value format.
static string ConvertPostgresUrl(string url)
{
    if (!url.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)) return url;
    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':', 2);
    var port = uri.Port > 0 ? uri.Port : 5432;
    return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};" +
           $"Username={Uri.UnescapeDataString(userInfo[0])};" +
           $"Password={(userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "")};" +
           "SSL Mode=Require;Trust Server Certificate=true";
}
