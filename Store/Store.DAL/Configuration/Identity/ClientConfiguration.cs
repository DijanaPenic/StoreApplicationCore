using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class ClientConfiguration : IEntityTypeConfiguration<ClientEntity> 
    {
        public void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            // Maps to the Client table
            builder.ToTable("client");

            // Primary key
            builder.HasKey(c => c.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(100);
            builder.Property(c => c.Secret).IsRequired();
            builder.Property(c => c.AllowedOrigin).HasMaxLength(100);

            // Set indeces
            builder.HasIndex(c => c.Name).HasName("NameIndex").IsUnique();
        }
    }
}