using FluentValidation;
using BookWormHub.ViewModels;

namespace BookWormHub.Validators;

public class BookCreateValidator : AbstractValidator<BookCreateViewModel>
{
    public BookCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên sách là bắt buộc")
            .MaximumLength(200).WithMessage("Tên sách tối đa 200 ký tự");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Tên tác giả là bắt buộc")
            .MaximumLength(100).WithMessage("Tên tác giả tối đa 100 ký tự");

        RuleFor(x => x.ISBN13)
            .NotEmpty().WithMessage("ISBN-13 là bắt buộc")
            .Length(13).WithMessage("ISBN phải đúng 13 ký tự")
            .Matches(@"^\d{13}$").WithMessage("ISBN chỉ chứa số")
            .Must(BeValidISBN13Checksum).WithMessage("ISBN-13 checksum không hợp lệ");

        RuleFor(x => x.Genre)
            .MaximumLength(50).WithMessage("Thể loại tối đa 50 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Mô tả tối đa 2000 ký tự");

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(10, DateTime.Now.Year)
            .When(x => x.PublishedYear.HasValue)
            .WithMessage(x => $"Năm xuất bản phải từ 10 đến {DateTime.Now.Year}");
    }

    private static bool BeValidISBN13Checksum(string? isbn)
    {
        if (string.IsNullOrEmpty(isbn) || isbn.Length != 13 || !isbn.All(char.IsDigit))
            return false;

        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            int digit = isbn[i] - '0';
            sum += (i % 2 == 0) ? digit : digit * 3;
        }
        return sum % 10 == 0;
    }
}
