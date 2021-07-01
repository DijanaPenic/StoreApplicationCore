using FluentValidation;

namespace Store.WebAPI.Models.Bookstore
{
    public class BookstorePatchApiModel
    {
        public string Name { get; set; }

        public string Location { get; set; }
    }

    public class BookstorePatchApiModelValidator : AbstractValidator<BookstorePatchApiModel>
    {
        public BookstorePatchApiModelValidator()
        {
            RuleFor(bs => bs.Name).NotEmpty().MaximumLength(50);
            RuleFor(bs => bs.Location).NotEmpty().MaximumLength(50);
        }
    }
}