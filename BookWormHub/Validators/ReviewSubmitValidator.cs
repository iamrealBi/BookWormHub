using FluentValidation;
using BookWormHub.ViewModels;

namespace BookWormHub.Validators;

public class ReviewSubmitValidator : AbstractValidator<ReviewSubmitViewModel>
{
    public ReviewSubmitValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0).WithMessage("BookId không hợp lệ");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating phải từ 1 đến 5");

        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Bình luận tối đa 2000 ký tự");
    }
}
