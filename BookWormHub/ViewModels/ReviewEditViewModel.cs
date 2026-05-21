namespace BookWormHub.ViewModels;

public class ReviewEditViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? StatusMessage { get; set; }
}
