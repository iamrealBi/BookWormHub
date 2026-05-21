namespace BookWormHub.ViewModels;

public class ReviewSubmitViewModel
{
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
