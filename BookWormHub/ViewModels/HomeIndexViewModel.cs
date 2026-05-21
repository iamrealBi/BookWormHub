using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class HomeIndexViewModel
{
    public int BookCount { get; set; }
    public int ReviewCount { get; set; }
    public int UserCount { get; set; }
    public List<Book> LatestBooks { get; set; } = new();
}
