using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Common.Enums;
using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class ClientConfiguration : IEntityTypeConfiguration<ClientEntity> 
    {
        public void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            // Maps to the Client table
            builder.ToTable("client", "identity");

            // Primary key
            builder.HasKey(c => c.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(100);
            builder.Property(c => c.Secret).IsRequired();
            builder.Property(c => c.AllowedOrigin).HasMaxLength(100);

            // Set indices
            builder.HasIndex(c => c.Name).HasDatabaseName("NameIndex").IsUnique();

            // Seed data
            builder.HasData
            (
                new ClientEntity
                {
                    Id = Guid.Parse("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                    Name = "WebApiApplication",
                    Description = "Web API Application",
                    Active = true,
                    AccessTokenLifeTime = 20,
                    RefreshTokenLifeTime = 60,
                    Secret = "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A==",
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationType.NativeConfidential,
                    DateCreatedUtc = DateTime.Parse("25-Oct-20 4:44:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("25-Oct-20 4:44:00 PM"),
                }
            );
        }
    }
}