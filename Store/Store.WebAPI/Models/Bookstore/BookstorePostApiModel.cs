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
            RuleFor(bs => bs.Name).NotEmpty().MaximumLength(50);
            RuleFor(bs => bs.Location).NotEmpty().MaximumLength(50);
        }
    }
}