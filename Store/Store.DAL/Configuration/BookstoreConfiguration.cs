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

            // Primary key
            builder.HasKey(bs => bs.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(bs => bs.Name).IsRequired().HasMaxLength(50);
            builder.Property(bs => bs.Location).IsRequired().HasMaxLength(100);
        }
    }
}