using BookWormHub.Models;
using BookWormHub.ViewModels;

namespace BookWormHub.Services.Interfaces;

public interface IBookService
{
    Task<BookIndexViewModel> GetBookListAsync(string? search, string? genre);
    Task<BookDetailsViewModel?> GetBookDetailsAsync(int id, string? userId);
    Task<Book?> GetBookForEditAsync(int id);
    Task<ServiceResult> CreateBookAsync(BookCreateViewModel model);
    Task<ServiceResult> UpdateBookAsync(int id, BookEditViewModel model);
    Task<ServiceResult> DeleteBookAsync(int id);
}
