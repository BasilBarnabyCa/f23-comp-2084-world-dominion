using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorldDominion.Models;
using WorldDominion.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add MySQL
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySQL(connectionString));

// Enabling Sessions
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Add Identity service and roles
builder.Services.AddDefaultIdentity<IdentityUser>()
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Add seeder
builder.Services.AddTransient<DbInitializer>();

// Register Cart Service as a new scoped dependency
builder.Services.AddScoped<CartService>();

var app = builder.Build();

// Turn on sessions
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Turns on authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "about",
	pattern: "about",
	defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
	name: "cart",
	pattern: "cart",
	defaults: new { controller = "Carts", action = "Index" });

app.MapControllerRoute(
	name: "privacy",
	pattern: "privacy",
	defaults: new { controller = "Home", action = "Privacy" });

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await DbInitializer.Initialize(
    scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>()
);

app.Run();
