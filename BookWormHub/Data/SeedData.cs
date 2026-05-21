using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookWormHub.Models;

namespace BookWormHub.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // ROLES
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ADMIN
        if (await userManager.FindByEmailAsync("admin@bookworm.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@bookworm.com",
                Email = "admin@bookworm.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // CLONE USER
        if (await userManager.FindByEmailAsync("user@bookworm.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "user@bookworm.com",
                Email = "user@bookworm.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "User123");
            await userManager.AddToRoleAsync(user, "User");
        }

        // SEED EXP BOOK
        if (!await context.Books.AnyAsync())
        {
            context.Books.AddRange(
                new Book
                {
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN13 = "9780132350884",
                    Genre = "Programming",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    PublishedYear = 2008
                },
                new Book
                {
                    Title = "The Pragmatic Programmer",
                    Author = "Andrew Hunt",
                    ISBN13 = "9780135957059",
                    Genre = "Programming",
                    Description = "Your Journey to Mastery, 20th Anniversary Edition",
                    PublishedYear = 2019
                },
                new Book
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    Author = "J.K. Rowling",
                    ISBN13 = "9780747532699",
                    Genre = "Fiction",
                    Description = "The first book in the Harry Potter series",
                    PublishedYear = 1997
                },
                new Book
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN13 = "9780061120084",
                    Genre = "Fiction",
                    Description = "A classic of modern American literature",
                    PublishedYear = 1960
                },
                new Book
                {
                    Title = "A Brief History of Time",
                    Author = "Stephen Hawking",
                    ISBN13 = "9780553380163",
                    Genre = "Science",
                    Description = "From the Big Bang to Black Holes",
                    PublishedYear = 1988
                }
            );
            await context.SaveChangesAsync();
        }

        // SEED EXP BADWORDs
        if (!await context.BannedWords.AnyAsync())
        {
            var seedDate = new DateTime(2026, 5, 3, 8, 15, 22, DateTimeKind.Utc);
            context.BannedWords.AddRange(
                new BannedWord { Word = "spam", CreatedAt = seedDate },
                new BannedWord { Word = "scam", CreatedAt = seedDate },
                new BannedWord { Word = "fake", CreatedAt = seedDate },
                new BannedWord { Word = "stupid", CreatedAt = seedDate },
                new BannedWord { Word = "trash", CreatedAt = seedDate }
            );
            await context.SaveChangesAsync();
        }
    }
}