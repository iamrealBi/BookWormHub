using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookWormHub.Services.Interfaces;
using BookWormHub.ViewModels;

namespace BookWormHub.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: /Review/Create/5
    public async Task<IActionResult> Create(int? id)
    {
        if (id == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var (viewModel, existingId) = await _reviewService.PrepareCreateAsync(id.Value, userId);

        if (existingId.HasValue)
            return RedirectToAction(nameof(Edit), new { id = existingId.Value });

        if (viewModel == null) return NotFound();

        return View(viewModel);
    }

    // POST: /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewSubmitViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var userId2 = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var (vm, _) = await _reviewService.PrepareCreateAsync(model.BookId, userId2!);
            return View(vm ?? new ReviewCreateViewModel { BookId = model.BookId, BookTitle = "" });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var result = await _reviewService.CreateOrUpdateReviewAsync(model, userId);

        return RedirectToAction("Details", "Book", new { id = result.Data, notify = result.Message });
    }

    // GET: /Review/Edit/3
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var viewModel = await _reviewService.PrepareEditAsync(id.Value, userId);
        if (viewModel == null) return NotFound();

        return View(viewModel);
    }

    // POST: /Review/Edit/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReviewSubmitViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var userId2 = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vm = await _reviewService.PrepareEditAsync(id, userId2!);
            return View(vm ?? new ReviewEditViewModel { Id = id, BookId = model.BookId });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var result = await _reviewService.UpdateReviewAsync(id, model, userId);
        if (!result.Success) return NotFound();

        return RedirectToAction("Details", "Book", new { id = model.BookId, notify = result.Message });
    }
}
