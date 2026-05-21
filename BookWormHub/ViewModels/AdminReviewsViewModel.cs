using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class AdminReviewsViewModel
{
    public List<Review> Reviews { get; set; } = new();
    public string? StatusMessage { get; set; }
}
