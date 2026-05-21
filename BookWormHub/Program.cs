using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using BookWormHub.Data;
using BookWormHub.Models;
using BookWormHub.Services;
using BookWormHub.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Register SERVICES

// MVC - Controllers + Views
builder.Services.AddControllersWithViews();

// Razor Pages (Identity UI)
builder.Services.AddRazorPages();

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// FluentValidation — auto-register all validators in assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Service Layer — interfaces mapped to implementations
builder.Services.AddScoped<IModerationService, ModerationService>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IHomeService, HomeService>();

var app = builder.Build();

// 2. MIDDLEWARE PIPELINE

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 3. ROUTES

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 4. SEED DATA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();

// Make Program class accessible for test project FluentValidation scanner
public partial class Program { }