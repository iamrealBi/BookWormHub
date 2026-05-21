using FluentValidation.TestHelper;
using BookWormHub.Validators;
using BookWormHub.ViewModels;

namespace BookWormHub.Tests.Validators;

public class BookCreateValidatorTests
{
    private readonly BookCreateValidator _validator = new();

    private static BookCreateViewModel ValidBook() => new()
    {
        Title = "Clean Code",
        Author = "Robert Martin",
        ISBN13 = "9780132350884",
        Genre = "Programming",
        PublishedYear = 2008
    };

    [Fact]
    public void Valid_Book_Passes()
    {
        var result = _validator.TestValidate(ValidBook());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Missing_Title_Fails()
    {
        var model = ValidBook();
        model.Title = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Missing_Author_Fails()
    {
        var model = ValidBook();
        model.Author = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Missing_ISBN_Fails()
    {
        var model = ValidBook();
        model.ISBN13 = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ISBN13);
    }

    [Fact]
    public void Invalid_ISBN_Checksum_Fails()
    {
        var model = ValidBook();
        model.ISBN13 = "1234567890123";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ISBN13);
    }

    [Fact]
    public void Invalid_PublishedYear_Fails()
    {
        var model = ValidBook();
        model.PublishedYear = 9999;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PublishedYear);
    }
}
