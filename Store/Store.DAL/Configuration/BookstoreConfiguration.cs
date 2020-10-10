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
            // Maps to the Book table
            builder.ToTable("bookstore");

            // Limit the size of columns to use efficient database types
            builder.Property(bs => bs.Name).IsRequired().HasMaxLength(50);
            builder.Property(bs => bs.Location).IsRequired().HasMaxLength(100);

            // Seed data
            builder.HasData
            (
                new BookstoreEntity
                {
                    Id = Guid.Parse("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                    Location = "2526 E Colfax Ave, Denver, WA",
                    Name = "Strand Book Store",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new BookstoreEntity
                {
                    Id = Guid.Parse("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"),
                    Location = "18325 Campus Way NE, Bothell, WA",
                    Name = "Powell's City of Books",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new BookstoreEntity
                {
                    Id = Guid.Parse("fa588725-0c60-4554-8eb9-20520038ee87"),
                    Location = "3415 SW Cedar Hills Blvd, Beaverton, OR",
                    Name = "Shakespeare & Co",
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                }
            );
        }
    }
}