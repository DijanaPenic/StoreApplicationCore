using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Common.Helpers;
using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class RoleConfiguration : IEntityTypeConfiguration<RoleEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Role");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .HasColumnType("varchar")
                   .HasMaxLength(256)
                   .IsRequired();

            builder.HasData
            (
                new RoleEntity 
                { 
                    Id = GuidHelper.NewSequentialGuid(), 
                    Name = "Admin",  
                    Stackable = false, 
                    DateCreatedUtc = 
                    DateTime.UtcNow, 
                    DateUpdatedUtc = DateTime.UtcNow 
                },
                new RoleEntity 
                { 
                    Id = GuidHelper.NewSequentialGuid(), 
                    Name = "Customer", 
                    Stackable = true, 
                    DateCreatedUtc = DateTime.UtcNow, 
                    DateUpdatedUtc = DateTime.UtcNow 
                },
                new RoleEntity 
                { 
                    Id = GuidHelper.NewSequentialGuid(), 
                    Name = "Store Manager", 
                    Stackable = true, 
                    DateCreatedUtc = DateTime.UtcNow, 
                    DateUpdatedUtc = DateTime.UtcNow 
                }
            );
        }
    }
}