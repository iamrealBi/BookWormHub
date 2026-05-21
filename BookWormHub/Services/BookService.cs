using Microsoft.EntityFrameworkCore;
using BookWormHub.Data;
using BookWormHub.Models;
using BookWormHub.ViewModels;
using BookWormHub.Services.Interfaces;

namespace BookWormHub.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BookIndexViewModel> GetBookListAsync(string? search, string? genre)
    {
        var query = _db.Books
            .Include(b => b.Reviews.Where(r => r.Status == ReviewStatus.Approved))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            query = query.Where(b =>
                b.Title.ToLower().Contains(s) ||
                b.Author.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        List<Book> books;
        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            books = await query
                .OrderByDescending(b => b.Title.ToLower() == s || b.Author.ToLower() == s)
                .ThenBy(b => b.Title)
                .ToListAsync();
        }
        else
        {
            books = await query.OrderBy(b => b.Title).ToListAsync();
        }

        var genres = await _db.Books
            .Select(b => b.Genre)
            .Where(g => g != "")
            .Distinct().OrderBy(g => g)
            .ToListAsync();

        return new BookIndexViewModel
        {
            Books = books,
            CurrentSearch = search,
            CurrentGenre = genre,
            Genres = genres
        };
    }

    public async Task<BookDetailsViewModel?> GetBookDetailsAsync(int id, string? userId)
    {
        var book = await _db.Books
            .Include(b => b.Reviews.Where(r => r.Status == ReviewStatus.Approved))
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return null;

        var approved = book.Reviews;
        double? avgRating = null;
        int reviewCount = 0;

        if (approved.Any())
        {
            avgRating = approved.Average(r => r.Rating);
            reviewCount = approved.Count;
        }

        Review? myReview = null;
        if (!string.IsNullOrEmpty(userId))
        {
            myReview = await _db.Reviews
                .FirstOrDefaultAsync(r => r.BookId == id && r.UserId == userId);
        }

        return new BookDetailsViewModel
        {
            Book = book,
            AvgRating = avgRating,
            ReviewCount = reviewCount,
            MyReview = myReview
        };
    }

    public async Task<Book?> GetBookForEditAsync(int id)
    {
        return await _db.Books.FindAsync(id);
    }

    public async Task<ServiceResult> CreateBookAsync(BookCreateViewModel model)
    {
        if (!string.IsNullOrEmpty(model.ISBN13) && !IsValidISBN13(model.ISBN13))
            return new ServiceResult { Success = false, ValidationErrors = { ["ISBN13"] = "ISBN-13 checksum không hợp lệ" } };

        if (await _db.Books.AnyAsync(b => b.ISBN13 == model.ISBN13))
            return new ServiceResult { Success = false, ValidationErrors = { ["ISBN13"] = "ISBN-13 đã tồn tại" } };

        var book = new Book
        {
            Title = model.Title,
            Author = model.Author,
            ISBN13 = model.ISBN13,
            Genre = model.Genre ?? string.Empty,
            Description = model.Description,
            PublishedYear = model.PublishedYear
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateBookAsync(int id, BookEditViewModel model)
    {
        if (id != model.Id)
            return ServiceResult.Fail("ID không khớp");

        if (!string.IsNullOrEmpty(model.ISBN13) && !IsValidISBN13(model.ISBN13))
            return new ServiceResult { Success = false, ValidationErrors = { ["ISBN13"] = "ISBN-13 checksum không hợp lệ" } };

        if (await _db.Books.AnyAsync(b => b.ISBN13 == model.ISBN13 && b.Id != id))
            return new ServiceResult { Success = false, ValidationErrors = { ["ISBN13"] = "ISBN-13 đã tồn tại" } };

        var book = await _db.Books.FindAsync(id);
        if (book == null) return ServiceResult.Fail("Sách không tồn tại");

        book.Title = model.Title;
        book.Author = model.Author;
        book.ISBN13 = model.ISBN13;
        book.Genre = model.Genre ?? string.Empty;
        book.Description = model.Description;
        book.PublishedYear = model.PublishedYear;

        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteBookAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book == null) return ServiceResult.Fail("Sách không tồn tại");

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public static bool IsValidISBN13(string isbn)
    {
        if (isbn.Length != 13 || !isbn.All(char.IsDigit))
            return false;

        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            int digit = isbn[i] - '0';
            sum += (i % 2 == 0) ? digit : digit * 3;
        }
        return sum % 10 == 0;
    }
}
