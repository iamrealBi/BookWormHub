using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;

namespace BookWormHub.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
