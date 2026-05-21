using BookWormHub.Models;
using BookWormHub.ViewModels;

namespace BookWormHub.Services.Interfaces;

public interface IReviewService
{
    Task<(ReviewCreateViewModel? ViewModel, int? ExistingReviewId)> PrepareCreateAsync(int bookId, string userId);
    Task<ServiceResult<int>> CreateOrUpdateReviewAsync(ReviewSubmitViewModel model, string userId);
    Task<ReviewEditViewModel?> PrepareEditAsync(int reviewId, string userId);
    Task<ServiceResult> UpdateReviewAsync(int reviewId, ReviewSubmitViewModel model, string userId);
}
