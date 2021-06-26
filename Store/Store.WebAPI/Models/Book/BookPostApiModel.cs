using System;
using FluentValidation;

namespace Store.WebAPI.Models.Book
{
    public class BookPostApiModel
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public Guid BookstoreId { get; set; }
    }

    public class BookPostApiModelValidator : AbstractValidator<BookPostApiModel>
    {
        public BookPostApiModelValidator()
        {
            RuleFor(book => book.Name).NotEmpty().MaximumLength(50);
            RuleFor(book => book.Author).NotEmpty().MaximumLength(50);
            RuleFor(book => book.BookstoreId).NotEmpty();
        }
    }
}