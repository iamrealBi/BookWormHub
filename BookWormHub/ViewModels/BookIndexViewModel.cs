using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class BookIndexViewModel
{
    public List<Book> Books { get; set; } = new();
    public string? CurrentSearch { get; set; }
    public string? CurrentGenre { get; set; }
    public List<string> Genres { get; set; } = new();
}
