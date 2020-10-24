using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class RoleConfiguration : IEntityTypeConfiguration<RoleEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            // Maps to the Role table
            builder.ToTable("role");

            // Primary key
            builder.HasKey(r => r.Id);

            // Index for "normalized" role name to allow efficient lookups
            builder.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();

            // A concurrency token for use with the optimistic concurrency checking
            builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            builder.Property(u => u.Name).IsRequired().HasMaxLength(256);
            builder.Property(u => u.NormalizedName).IsRequired().HasMaxLength(256);

            // The relationships between Role and other entity types
            // Note that these relationships are configured with no navigation properties

            // Each Role can have many entries in the UserRole join table
            builder.HasMany<UserRoleEntity>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

            // Each Role can have many associated RoleClaims
            builder.HasMany<RoleClaimEntity>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();

            // Seed data
            builder.HasData
            (
                new RoleEntity
                {
                    Id = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Stackable = false,
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new RoleEntity
                {
                    Id = Guid.Parse("d82ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    Stackable = true,
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                },
                new RoleEntity
                {
                    Id = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    Name = "Store Manager",
                    NormalizedName = "STORE MANAGER",
                    Stackable = true,
                    DateCreatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("08-Oct-20 6:44:11 PM")
                }
            );
        }
    }
}