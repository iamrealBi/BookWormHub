using BookWormHub.Models;

namespace BookWormHub.ViewModels;

public class AdminBannedWordsViewModel
{
    public List<BannedWord> Words { get; set; } = new();
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
