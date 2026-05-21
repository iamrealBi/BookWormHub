using FluentAssertions;
using BookWormHub.Models;
using BookWormHub.Services;
using BookWormHub.Tests.Helpers;

namespace BookWormHub.Tests.Services;

public class ModerationServiceTests
{
    [Fact]
    public async Task ContainsBannedWord_NullText_ReturnsFalse()
    {
        var db = TestDbContextFactory.Create();
        var sut = new ModerationService(db);
        (await sut.ContainsBannedWord(null)).Should().BeFalse();
    }

    [Fact]
    public async Task ContainsBannedWord_EmptyText_ReturnsFalse()
    {
        var db = TestDbContextFactory.Create();
        var sut = new ModerationService(db);
        (await sut.ContainsBannedWord("")).Should().BeFalse();
    }

    [Fact]
    public async Task ContainsBannedWord_CleanText_ReturnsFalse()
    {
        var db = TestDbContextFactory.Create();
        db.BannedWords.Add(new BannedWord { Word = "spam" });
        await db.SaveChangesAsync();
        var sut = new ModerationService(db);
        (await sut.ContainsBannedWord("This is a clean comment")).Should().BeFalse();
    }

    [Fact]
    public async Task ContainsBannedWord_TextWithBannedWord_ReturnsTrue()
    {
        var db = TestDbContextFactory.Create();
        db.BannedWords.Add(new BannedWord { Word = "spam" });
        await db.SaveChangesAsync();
        var sut = new ModerationService(db);
        (await sut.ContainsBannedWord("This is spam content")).Should().BeTrue();
    }

    [Fact]
    public async Task ContainsBannedWord_CaseInsensitive_ReturnsTrue()
    {
        var db = TestDbContextFactory.Create();
        db.BannedWords.Add(new BannedWord { Word = "spam" });
        await db.SaveChangesAsync();
        var sut = new ModerationService(db);
        (await sut.ContainsBannedWord("This is SPAM content")).Should().BeTrue();
    }
}
