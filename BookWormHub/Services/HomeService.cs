using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;
using BookWormHub.Models;
using BookWormHub.ViewModels;
using BookWormHub.Services.Interfaces;

namespace BookWormHub.Services;

public class HomeService : IHomeService
{
    private readonly AppDbContext _db;

    public HomeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<HomeIndexViewModel> GetDashboardAsync()
    {
        return new HomeIndexViewModel
        {
            BookCount = await _db.Books.CountAsync(),
            ReviewCount = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Approved),
            UserCount = await _db.Users.CountAsync(),
            LatestBooks = await _db.Books.OrderByDescending(b => b.Id).Take(3).ToListAsync()
        };
    }

    public async Task<ProfileViewModel?> GetProfileAsync(string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        var reviews = await _db.Reviews
            .Include(r => r.Book)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return new ProfileViewModel
        {
            User = user,
            Reviews = reviews
        };
    }
}
