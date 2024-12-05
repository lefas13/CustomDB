using Microsoft.EntityFrameworkCore;
using CustomMVC.Service;
using CustomMVC.Middleware;
using CustomMVC.Data;
using Microsoft.AspNetCore.Identity;
using CustomMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

builder.Services.AddDbContext<CustomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
string connectionUsers = builder.Configuration.GetConnectionString("IdentityConnection");
// Настройка контекста для Identity (пользователи, роли, аутентификация и т.д.)
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionUsers));
// Настройка Identity с использованием ApplicationDbContext для аутентификации
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseDbInitializer();

// Использование Identity
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Добавьте явный маршрут для вашего контроллера, если нужно
    endpoints.MapControllerRoute(
        name: "customInfo",
        pattern: "CustomInformation/{action=Index}/{id?}",
        defaults: new { controller = "CustomInformation" });
});

app.MapRazorPages();

app.Run();
