using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class BookDetailsViewModel
{
    public Book Book { get; set; } = null!;
    public double? AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public Review? MyReview { get; set; }
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
