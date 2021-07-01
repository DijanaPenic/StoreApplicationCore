using FluentValidation;
using System;

namespace Store.WebAPI.Models.Book
{
    public class BookPatchApiModel
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public Guid BookstoreId { get; set; }
    }

    public class BookPatchApiModelValidator : AbstractValidator<BookPatchApiModel>
    {
        public BookPatchApiModelValidator()
        {
            RuleFor(b => b.Name).NotEmpty().MaximumLength(50);
            RuleFor(b => b.Author).NotEmpty().MaximumLength(50);
            RuleFor(b => b.BookstoreId).NotEmpty();
        }
    }
}