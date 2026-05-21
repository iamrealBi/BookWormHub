namespace BookWormHub.ViewModels;

public class BookEditViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN13 { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublishedYear { get; set; }
}
