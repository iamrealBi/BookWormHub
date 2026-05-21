using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;

namespace BookWormHub.Services
{
    public class ModerationService : Interfaces.IModerationService
    {
        private readonly AppDbContext _db;

        public ModerationService(AppDbContext db)
        {
            _db = db;
        }

        // Check if text contains any banned word (case insensitive, substring match)
        public async Task<bool> ContainsBannedWord(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var bannedWords = await _db.BannedWords
                .Select(bw => bw.Word.ToLower())
                .ToListAsync();

            string lower = text.ToLower();
            return bannedWords.Any(w => lower.Contains(w));
        }
    }
}
