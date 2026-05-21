using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookWormHub.Services.Interfaces;
using BookWormHub.ViewModels;

namespace BookWormHub.Controllers;

public class BookController : Controller
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: /Book?search=clean&genre=Programming
    public async Task<IActionResult> Index(string? search, string? genre)
    {
        var model = await _bookService.GetBookListAsync(search, genre);
        return View(model);
    }

    // GET: /Book/Details/5
    public async Task<IActionResult> Details(int? id, string? notify)
    {
        if (id == null) return NotFound();

        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;

        var model = await _bookService.GetBookDetailsAsync(id.Value, userId);
        if (model == null) return NotFound();

        // Read notification from query string (replaces TempData)
        if (!string.IsNullOrEmpty(notify))
            model.StatusMessage = notify;

        return View(model);
    }

    // GET: /Book/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new BookCreateViewModel());
    }

    // POST: /Book/Create
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _bookService.CreateBookAsync(model);
        if (!result.Success)
        {
            foreach (var err in result.ValidationErrors)
                ModelState.AddModelError(err.Key, err.Value);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Book/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _bookService.GetBookForEditAsync(id.Value);
        if (book == null) return NotFound();

        var model = new BookEditViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN13 = book.ISBN13,
            Genre = book.Genre,
            Description = book.Description,
            PublishedYear = book.PublishedYear
        };

        return View(model);
    }

    // POST: /Book/Edit/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _bookService.UpdateBookAsync(id, model);
        if (!result.Success)
        {
            if (!string.IsNullOrEmpty(result.Error))
                return NotFound();

            foreach (var err in result.ValidationErrors)
                ModelState.AddModelError(err.Key, err.Value);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Book/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var book = await _bookService.GetBookForEditAsync(id.Value);
        if (book == null) return NotFound();
        return View(book);
    }

    // POST: /Book/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
