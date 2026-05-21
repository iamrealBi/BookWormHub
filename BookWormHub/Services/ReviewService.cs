using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;
using BookWormHub.Models;
using BookWormHub.ViewModels;
using BookWormHub.Services.Interfaces;

namespace BookWormHub.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;
    private readonly IModerationService _moderation;
    private readonly IBadgeService _badge;

    public ReviewService(AppDbContext db, IModerationService moderation, IBadgeService badge)
    {
        _db = db;
        _moderation = moderation;
        _badge = badge;
    }

    public async Task<(ReviewCreateViewModel? ViewModel, int? ExistingReviewId)> PrepareCreateAsync(int bookId, string userId)
    {
        var book = await _db.Books.FindAsync(bookId);
        if (book == null) return (null, null);

        var existing = await _db.Reviews
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

        if (existing != null)
            return (null, existing.Id);

        return (new ReviewCreateViewModel
        {
            BookId = book.Id,
            BookTitle = book.Title
        }, null);
    }

    public async Task<ServiceResult<int>> CreateOrUpdateReviewAsync(ReviewSubmitViewModel model, string userId)
    {
        var existing = await _db.Reviews
            .FirstOrDefaultAsync(r => r.BookId == model.BookId && r.UserId == userId);

        if (existing != null)
        {
            existing.Rating = model.Rating;
            existing.Comment = model.Comment;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.Status = await _moderation.ContainsBannedWord(model.Comment)
                ? ReviewStatus.Hidden : ReviewStatus.Approved;

            await _db.SaveChangesAsync();

            if (existing.Status == ReviewStatus.Approved)
                await _badge.CheckAndAwardBadge(userId);

            var msg = existing.Status == ReviewStatus.Hidden
                ? "Review đang chờ kiểm duyệt." : "Review đã được cập nhật!";
            return ServiceResult<int>.Ok(model.BookId, msg);
        }

        var status = await _moderation.ContainsBannedWord(model.Comment)
            ? ReviewStatus.Hidden : ReviewStatus.Approved;

        var review = new Review
        {
            BookId = model.BookId,
            UserId = userId,
            Rating = model.Rating,
            Comment = model.Comment,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        if (status == ReviewStatus.Approved)
            await _badge.CheckAndAwardBadge(userId);

        var message = status == ReviewStatus.Hidden
            ? "Review đang chờ kiểm duyệt." : "Review đã được gửi!";
        return ServiceResult<int>.Ok(model.BookId, message);
    }

    public async Task<ReviewEditViewModel?> PrepareEditAsync(int reviewId, string userId)
    {
        var review = await _db.Reviews
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

        if (review == null) return null;

        return new ReviewEditViewModel
        {
            Id = review.Id,
            BookId = review.BookId,
            BookTitle = review.Book?.Title ?? "",
            Rating = review.Rating,
            Comment = review.Comment
        };
    }

    public async Task<ServiceResult> UpdateReviewAsync(int reviewId, ReviewSubmitViewModel model, string userId)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

        if (review == null)
            return ServiceResult.Fail("Review không tồn tại");

        review.Rating = model.Rating;
        review.Comment = model.Comment;
        review.UpdatedAt = DateTime.UtcNow;
        review.Status = await _moderation.ContainsBannedWord(model.Comment)
            ? ReviewStatus.Hidden : ReviewStatus.Approved;

        await _db.SaveChangesAsync();

        if (review.Status == ReviewStatus.Approved)
            await _badge.CheckAndAwardBadge(userId);

        var message = review.Status == ReviewStatus.Hidden
            ? "Review đang chờ kiểm duyệt." : "Review đã được cập nhật!";
        return ServiceResult.Ok(message);
    }
}
