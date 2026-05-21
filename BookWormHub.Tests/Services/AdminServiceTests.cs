using FluentAssertions;
using NSubstitute;
using BookWormHub.Models;
using BookWormHub.Services;
using BookWormHub.Services.Interfaces;
using BookWormHub.Tests.Helpers;

namespace BookWormHub.Tests.Services;

public class AdminServiceTests
{
    private AdminService CreateService(out Data.AppDbContext db, IBadgeService? badge = null)
    {
        db = TestDbContextFactory.Create();
        badge ??= Substitute.For<IBadgeService>();
        return new AdminService(db, badge);
    }

    [Fact]
    public async Task AddBannedWordAsync_Success()
    {
        var sut = CreateService(out var db);
        var result = await sut.AddBannedWordAsync("spam");
        result.Success.Should().BeTrue();
        db.BannedWords.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddBannedWordAsync_Empty_Fails()
    {
        var sut = CreateService(out _);
        var result = await sut.AddBannedWordAsync("");
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task AddBannedWordAsync_Duplicate_Fails()
    {
        var sut = CreateService(out var db);
        db.BannedWords.Add(new BannedWord { Word = "spam" });
        await db.SaveChangesAsync();

        var result = await sut.AddBannedWordAsync("SPAM");
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteBannedWordAsync_Success()
    {
        var sut = CreateService(out var db);
        db.BannedWords.Add(new BannedWord { Word = "test" });
        await db.SaveChangesAsync();
        var wordId = db.BannedWords.First().Id;

        var result = await sut.DeleteBannedWordAsync(wordId);
        result.Success.Should().BeTrue();
        db.BannedWords.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteBannedWordAsync_NotFound_Fails()
    {
        var sut = CreateService(out _);
        var result = await sut.DeleteBannedWordAsync(999);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ApproveReviewAsync_Success_ChecksBadge()
    {
        var badge = Substitute.For<IBadgeService>();
        var sut = CreateService(out var db, badge);
        db.Reviews.Add(new Review { BookId = 1, UserId = "u1", Rating = 5, Status = ReviewStatus.Hidden });
        await db.SaveChangesAsync();
        var reviewId = db.Reviews.First().Id;

        var result = await sut.ApproveReviewAsync(reviewId);

        result.Success.Should().BeTrue();
        db.Reviews.First().Status.Should().Be(ReviewStatus.Approved);
        await badge.Received(1).CheckAndAwardBadge("u1");
    }

    [Fact]
    public async Task RejectReviewAsync_Success()
    {
        var sut = CreateService(out var db);
        db.Reviews.Add(new Review { BookId = 1, UserId = "u1", Rating = 3, Status = ReviewStatus.Hidden });
        await db.SaveChangesAsync();
        var reviewId = db.Reviews.First().Id;

        var result = await sut.RejectReviewAsync(reviewId);

        result.Success.Should().BeTrue();
        db.Reviews.First().Status.Should().Be(ReviewStatus.Rejected);
    }

    [Fact]
    public async Task RevokeBadgeAsync_Success()
    {
        var sut = CreateService(out var db);
        db.Users.Add(new ApplicationUser { Id = "u1", UserName = "test", Email = "t@t.com", IsCritic = true, CrticSince = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var result = await sut.RevokeBadgeAsync("u1");

        result.Success.Should().BeTrue();
        db.Users.First().IsCritic.Should().BeFalse();
    }
}
