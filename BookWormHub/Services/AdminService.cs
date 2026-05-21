using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;
using BookWormHub.Models;
using BookWormHub.ViewModels;
using BookWormHub.Services.Interfaces;

namespace BookWormHub.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly IBadgeService _badge;

    public AdminService(AppDbContext db, IBadgeService badge)
    {
        _db = db;
        _badge = badge;
    }

    public async Task<AdminBannedWordsViewModel> GetBannedWordsAsync()
    {
        var words = await _db.BannedWords.OrderBy(w => w.Word).ToListAsync();
        return new AdminBannedWordsViewModel { Words = words };
    }

    public async Task<ServiceResult> AddBannedWordAsync(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return ServiceResult.Fail("Từ cấm không được để trống");

        word = word.Trim().ToLower();

        if (await _db.BannedWords.AnyAsync(bw => bw.Word.ToLower() == word))
            return ServiceResult.Fail($"Từ '{word}' đã có trong danh sách");

        _db.BannedWords.Add(new BannedWord { Word = word, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return ServiceResult.Ok($"Đã thêm từ cấm: '{word}'");
    }

    public async Task<ServiceResult> DeleteBannedWordAsync(int id)
    {
        var word = await _db.BannedWords.FindAsync(id);
        if (word == null) return ServiceResult.Fail("Từ cấm không tồn tại");

        _db.BannedWords.Remove(word);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok($"Đã xóa từ cấm: '{word.Word}'");
    }

    public async Task<AdminReviewsViewModel> GetHiddenReviewsAsync()
    {
        var reviews = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .Where(r => r.Status == ReviewStatus.Hidden)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return new AdminReviewsViewModel { Reviews = reviews };
    }

    public async Task<ServiceResult> ApproveReviewAsync(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return ServiceResult.Fail("Review không tồn tại");

        review.Status = ReviewStatus.Approved;
        await _db.SaveChangesAsync();
        await _badge.CheckAndAwardBadge(review.UserId);
        return ServiceResult.Ok("Đã duyệt review!");
    }

    public async Task<ServiceResult> RejectReviewAsync(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return ServiceResult.Fail("Review không tồn tại");

        review.Status = ReviewStatus.Rejected;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok("Đã từ chối review!");
    }

    public async Task<ServiceResult> RevokeBadgeAsync(string userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return ServiceResult.Fail("User không tồn tại");

        user.IsCritic = false;
        user.CrticSince = null;
        await _db.SaveChangesAsync();
        return ServiceResult.Ok($"Đã thu hồi badge của {user.UserName}");
    }
}
