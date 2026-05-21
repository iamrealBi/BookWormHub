namespace BookWormHub.Models;

public enum ReviewStatus
{
    Approved,
    Hidden,
    Rejected
}

public class Review
{
    public int Id { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Approved;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }
}
