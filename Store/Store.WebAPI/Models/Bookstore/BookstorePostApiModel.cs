using FluentValidation;

namespace Store.WebAPI.Models.Bookstore
{
    public class BookstorePostApiModel
    {
        public string Name { get; set; }

        public string Location { get; set; }
    }

    public class BookstorePostApiModelValidator : AbstractValidator<BookstorePostApiModel>
    {
        public BookstorePostApiModelValidator()
        {
            RuleFor(bookstore => bookstore.Name).NotEmpty().MaximumLength(50);
            RuleFor(bookstore => bookstore.Location).NotEmpty().MaximumLength(50);
        }
    }
}