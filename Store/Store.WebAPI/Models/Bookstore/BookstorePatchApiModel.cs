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
            RuleFor(bookstore => bookstore.Name).NotEmpty().MaximumLength(50);
            RuleFor(bookstore => bookstore.Location).NotEmpty().MaximumLength(50);
        }
    }
}