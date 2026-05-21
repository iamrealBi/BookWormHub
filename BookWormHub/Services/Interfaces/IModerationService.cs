namespace BookWormHub.Services.Interfaces;

public interface IModerationService
{
    Task<bool> ContainsBannedWord(string? text);
}
