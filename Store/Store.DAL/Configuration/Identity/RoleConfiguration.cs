﻿using System;
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
            builder.ToTable("role", "identity");

            // Primary key
            builder.HasKey(r => r.Id);

            // Index for "normalized" role name to allow efficient lookups
            builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

            // A concurrency token for use with the optimistic concurrency checking
            builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
            builder.Property(r => r.NormalizedName).IsRequired().HasMaxLength(256);

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
                },
                new RoleEntity
                {
                    Id = Guid.Parse("9621c09c-06b1-45fb-8baf-38e0757e2f59"),
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    Stackable = false,
                    DateCreatedUtc = DateTime.Parse("30-Oct-20 6:44:11 PM"),
                    DateUpdatedUtc = DateTime.Parse("30-Oct-20 6:44:11 PM")
                }
            );
        }
    }
}