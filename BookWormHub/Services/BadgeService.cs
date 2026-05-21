using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;
using BookWormHub.Models;

namespace BookWormHub.Services
{
    public class BadgeService : Interfaces.IBadgeService
    {
        private readonly AppDbContext _db;
        private const int CRITIC_THRESHOLD = 10; // 10 approved reviews = Critic

        public BadgeService(AppDbContext db)
        {
            _db = db;
        }

        // Auto award Critic badge when user reaches threshold
        public async Task CheckAndAwardBadge(string userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.IsCritic) return; // Already critic, skip

            int count = await _db.Reviews
                .CountAsync(r => r.UserId == userId && r.Status == ReviewStatus.Approved);

            if (count >= CRITIC_THRESHOLD)
            {
                user.IsCritic = true;
                user.CrticSince = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }
}
