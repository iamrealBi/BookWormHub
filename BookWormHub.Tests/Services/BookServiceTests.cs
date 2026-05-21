using FluentAssertions;
using BookWormHub.Models;
using BookWormHub.Services;
using BookWormHub.ViewModels;
using BookWormHub.Tests.Helpers;

namespace BookWormHub.Tests.Services;

public class BookServiceTests
{
    private BookService CreateService(out Data.AppDbContext db)
    {
        db = TestDbContextFactory.Create();
        return new BookService(db);
    }

    [Fact]
    public async Task GetBookListAsync_ReturnsAllBooks()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Title = "A", Author = "X", ISBN13 = "9780132350884", Genre = "Fiction" });
        db.Books.Add(new Book { Title = "B", Author = "Y", ISBN13 = "9781234567897", Genre = "Science" });
        await db.SaveChangesAsync();

        var result = await sut.GetBookListAsync(null, null);

        result.Books.Should().HaveCount(2);
        result.Genres.Should().Contain("Fiction");
    }

    [Fact]
    public async Task GetBookListAsync_SearchFiltersResults()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Title = "Clean Code", Author = "Martin", ISBN13 = "9780132350884", Genre = "Programming" });
        db.Books.Add(new Book { Title = "Other Book", Author = "Smith", ISBN13 = "9781234567897", Genre = "Fiction" });
        await db.SaveChangesAsync();

        var result = await sut.GetBookListAsync("Clean", null);

        result.Books.Should().HaveCount(1);
        result.Books[0].Title.Should().Be("Clean Code");
    }

    [Fact]
    public async Task GetBookListAsync_GenreFiltersResults()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Title = "A", Author = "X", ISBN13 = "9780132350884", Genre = "Fiction" });
        db.Books.Add(new Book { Title = "B", Author = "Y", ISBN13 = "9781234567897", Genre = "Science" });
        await db.SaveChangesAsync();

        var result = await sut.GetBookListAsync(null, "Fiction");

        result.Books.Should().HaveCount(1);
        result.CurrentGenre.Should().Be("Fiction");
    }

    [Fact]
    public async Task GetBookDetailsAsync_ReturnsNull_WhenNotFound()
    {
        var sut = CreateService(out _);
        var result = await sut.GetBookDetailsAsync(999, null);
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookAsync_Success()
    {
        var sut = CreateService(out var db);
        var model = new BookCreateViewModel { Title = "Test", Author = "Auth", ISBN13 = "9780132350884", Genre = "Programming" };

        var result = await sut.CreateBookAsync(model);

        result.Success.Should().BeTrue();
        db.Books.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateBookAsync_DuplicateISBN_ReturnsError()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Title = "Existing", Author = "Auth", ISBN13 = "9780132350884", Genre = "Programming" });
        await db.SaveChangesAsync();

        var model = new BookCreateViewModel { Title = "New", Author = "Auth", ISBN13 = "9780132350884", Genre = "Fiction" };
        var result = await sut.CreateBookAsync(model);

        result.Success.Should().BeFalse();
        result.ValidationErrors.Should().ContainKey("ISBN13");
    }

    [Fact]
    public async Task CreateBookAsync_InvalidChecksum_ReturnsError()
    {
        var sut = CreateService(out _);
        var model = new BookCreateViewModel { Title = "Test", Author = "Auth", ISBN13 = "1234567890123", Genre = "Fiction" };

        var result = await sut.CreateBookAsync(model);

        result.Success.Should().BeFalse();
        result.ValidationErrors.Should().ContainKey("ISBN13");
    }

    [Fact]
    public async Task DeleteBookAsync_Success()
    {
        var sut = CreateService(out var db);
        db.Books.Add(new Book { Title = "Del", Author = "A", ISBN13 = "9780132350884", Genre = "Fiction" });
        await db.SaveChangesAsync();
        var bookId = db.Books.First().Id;

        var result = await sut.DeleteBookAsync(bookId);

        result.Success.Should().BeTrue();
        db.Books.Should().BeEmpty();
    }

    [Theory]
    [InlineData("9780132350884", true)]
    [InlineData("1234567890123", false)]
    [InlineData("978013235088", false)]
    [InlineData("abcdefghijklm", false)]
    public void IsValidISBN13_ReturnsExpected(string isbn, bool expected)
    {
        BookService.IsValidISBN13(isbn).Should().Be(expected);
    }
}
