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
        }
    }
}