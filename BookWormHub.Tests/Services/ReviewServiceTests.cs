using FluentAssertions;
using NSubstitute;
using BookWormHub.Models;
using BookWormHub.Services;
using BookWormHub.Services.Interfaces;
using BookWormHub.ViewModels;
using BookWormHub.Tests.Helpers;

namespace BookWormHub.Tests.Services;

public class ReviewServiceTests
{
    private ReviewService CreateService(out Data.AppDbContext db, IModerationService? moderation = null, IBadgeService? badge = null)
    {
        db = TestDbContextFactory.Create();
        moderation ??= Substitute.For<IModerationService>();
        badge ??= Substitute.For<IBadgeService>();
        moderation.ContainsBannedWord(Arg.Any<string?>()).Returns(false);
        return new ReviewService(db, moderation, badge);
    }

    [Fact]
    public async Task PrepareCreateAsync_BookNotFound_ReturnsNull()
    {
        var sut = CreateService(out _);
        var (vm, id) = await sut.PrepareCreateAsync(999, "user1");
        vm.Should().BeNull();
        id.Should().BeNull();
    }

    [Fact]
    public async Task PrepareCreateAsync_AlreadyReviewed_ReturnsExistingId()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Id = 1, Title = "T", Author = "A", ISBN13 = "9780132350884", Genre = "F" });
        db.Reviews.Add(new Review { Id = 10, BookId = 1, UserId = "user1", Rating = 5 });
        await db.SaveChangesAsync();

        var (vm, id) = await sut.PrepareCreateAsync(1, "user1");
        vm.Should().BeNull();
        id.Should().Be(10);
    }

    [Fact]
    public async Task PrepareCreateAsync_Success_ReturnsViewModel()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Id = 1, Title = "Test Book", Author = "A", ISBN13 = "9780132350884", Genre = "F" });
        await db.SaveChangesAsync();

        var (vm, id) = await sut.PrepareCreateAsync(1, "user1");
        vm.Should().NotBeNull();
        vm!.BookTitle.Should().Be("Test Book");
        id.Should().BeNull();
    }

    [Fact]
    public async Task CreateOrUpdateReviewAsync_NewReview_Success()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Id = 1, Title = "T", Author = "A", ISBN13 = "9780132350884", Genre = "F" });
        await db.SaveChangesAsync();

        var model = new ReviewSubmitViewModel { BookId = 1, Rating = 4, Comment = "Great" };
        var result = await sut.CreateOrUpdateReviewAsync(model, "user1");

        result.Success.Should().BeTrue();
        result.Data.Should().Be(1);
        db.Reviews.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOrUpdateReviewAsync_BannedWord_StatusHidden()
    {
        var moderation = Substitute.For<IModerationService>();
        moderation.ContainsBannedWord(Arg.Any<string?>()).Returns(true);
        var badge = Substitute.For<IBadgeService>();

        var db = TestDbContextFactory.Create();
        db.Books.Add(new Book { Id = 1, Title = "T", Author = "A", ISBN13 = "9780132350884", Genre = "F" });
        await db.SaveChangesAsync();

        var sut = new ReviewService(db, moderation, badge);
        var model = new ReviewSubmitViewModel { BookId = 1, Rating = 3, Comment = "spam" };
        var result = await sut.CreateOrUpdateReviewAsync(model, "user1");

        result.Success.Should().BeTrue();
        var review = db.Reviews.First();
        review.Status.Should().Be(ReviewStatus.Hidden);
    }

    [Fact]
    public async Task UpdateReviewAsync_ReviewNotFound_Fails()
    {
        var sut = CreateService(out _);
        var model = new ReviewSubmitViewModel { BookId = 1, Rating = 3, Comment = "test" };
        var result = await sut.UpdateReviewAsync(999, model, "user1");
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateReviewAsync_Success()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Id = 1, Title = "T", Author = "A", ISBN13 = "9780132350884", Genre = "F" });
        db.Reviews.Add(new Review { Id = 1, BookId = 1, UserId = "user1", Rating = 3, Comment = "old" });
        await db.SaveChangesAsync();

        var model = new ReviewSubmitViewModel { BookId = 1, Rating = 5, Comment = "updated" };
        var result = await sut.UpdateReviewAsync(1, model, "user1");

        result.Success.Should().BeTrue();
        db.Reviews.First().Rating.Should().Be(5);
        db.Reviews.First().Comment.Should().Be("updated");
    }
}
