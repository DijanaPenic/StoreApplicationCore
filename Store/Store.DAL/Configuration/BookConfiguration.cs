using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities;

namespace Store.DAL.Configuration
{
    internal class BookConfiguration : IEntityTypeConfiguration<BookEntity> 
    {
        public void Configure(EntityTypeBuilder<BookEntity> builder)
        {
            Guid firstBookstoreId = Guid.Parse("61c048ca-028d-4466-b7fd-4a05f0dad647");

            builder.HasData
            (
                new BookEntity
                {
                    Id = Guid.Parse("304e51fa-e2dc-4114-bf3f-08d86b04996d"),
                    BookstoreId = firstBookstoreId,
                    Author = "J. R. R. Tolkien",
                    Name = "The Lord of the Rings",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new BookEntity
                {
                    Id = Guid.Parse("2d59e1d6-05e8-47a4-bf40-08d86b04996d"),
                    BookstoreId = firstBookstoreId,
                    Author = "Paulo Coelho",
                    Name = "The Alchemist",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new BookEntity
                {
                    Id = Guid.Parse("53b9c986-1857-4878-bf41-08d86b04996d"),
                    BookstoreId = firstBookstoreId,
                    Author = "Antoine de Saint-Exupéry",
                    Name = "The Little Prince",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                }
            );
        }
    }
}