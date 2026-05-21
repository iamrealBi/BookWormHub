namespace BookWormHub.ViewModels;

public class ReviewCreateViewModel
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
