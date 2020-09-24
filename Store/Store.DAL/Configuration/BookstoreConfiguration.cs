using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities;

namespace Store.DAL.Configuration
{
    internal class BookstoreConfiguration : IEntityTypeConfiguration<BookstoreEntity> 
    {
        public void Configure(EntityTypeBuilder<BookstoreEntity> builder)
        {
            builder.HasData
            (
                new BookstoreEntity
                {
                    Id = Guid.Parse("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                    Location = "2526 E Colfax Ave, Denver, WA",
                    Name = "Strand Book Store",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow
                },
                new BookstoreEntity
                {
                    Id = Guid.Parse("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"),
                    Location = "18325 Campus Way NE, Bothell, WA",
                    Name = "Powell's City of Books",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow
                },
                new BookstoreEntity
                {
                    Id = Guid.Parse("fa588725-0c60-4554-8eb9-20520038ee87"),
                    Location = "3415 SW Cedar Hills Blvd, Beaverton, OR",
                    Name = "Shakespeare & Co",
                    DateCreatedUtc = DateTime.UtcNow,
                    DateUpdatedUtc = DateTime.UtcNow
                }
            );
        }
    }
}