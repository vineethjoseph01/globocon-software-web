using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using GloboconSoftwareWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure large file upload limits (up to 500 MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500 MB
    options.ValueLengthLimit = 524288000;
    options.MultipartHeadersLengthLimit = 524288000;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000; // 500 MB
});

// Database configuration with automatic fallback to SQLite if LocalDB is unavailable
var sqlServerConn = builder.Configuration.GetConnectionString("DefaultConnection");
var sqliteConn = builder.Configuration.GetConnectionString("SqliteConnection");

bool isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
bool isLocalDbConn = !string.IsNullOrEmpty(sqlServerConn) && sqlServerConn.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);

if (isAzure && isLocalDbConn)
{
    // On Azure App Service, LocalDB is unavailable; use SQLite for instant 0-second startup
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConn ?? "Data Source=globocon.db"));
}
else
{
    try
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlServerConn, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(2), errorNumbersToAdd: null);
            }));
    }
    catch
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(sqliteConn ?? "Data Source=globocon.db"));
    }
}

// Cookie Authentication for Admin Panel
builder.Services.AddAuthentication("GloboconAdminAuth")
    .AddCookie("GloboconAdminAuth", options =>
    {
        options.Cookie.Name = "GloboconAdmin.AuthToken";
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Ensure Database & Migrations/Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        try
        {
            if (dbContext.Database.IsSqlite())
            {
                dbContext.Database.EnsureCreated();
            }
            else
            {
                dbContext.Database.Migrate();
            }
        }
        catch
        {
            dbContext.Database.EnsureCreated();
        }

        DbInitializer.Initialize(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
