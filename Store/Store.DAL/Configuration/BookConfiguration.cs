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
            // Maps to the Book table
            builder.ToTable("book");

            // Primary key
            builder.HasKey(b => b.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
            builder.Property(b => b.Author).IsRequired().HasMaxLength(50);

            // The relationships between Book and Bookstores
            // Each Book can have one Bookstore
            builder.HasOne(b => b.Bookstore).WithMany(bs => bs.Books).HasForeignKey(b => b.BookstoreId).IsRequired();

            // Seed data
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