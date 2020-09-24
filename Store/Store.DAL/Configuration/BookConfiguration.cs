using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities;
using Store.Common.Helpers;

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
                    Id = GuidHelper.NewSequentialGuid(),
                    BookstoreId = firstBookstoreId,
                    Author = "J. R. R. Tolkien",
                    Name = "The Lord of the Rings",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow
                },
                new BookEntity
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    BookstoreId = firstBookstoreId,
                    Author = "Paulo Coelho",
                    Name = "The Alchemist",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow
                },
                new BookEntity
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    BookstoreId = firstBookstoreId,
                    Author = "Antoine de Saint-Exupéry",
                    Name = "The Little Prince",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow,
                }
            );
        }
    }
}