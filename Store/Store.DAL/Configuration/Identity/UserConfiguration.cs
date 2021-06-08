using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserConfiguration : IEntityTypeConfiguration<UserEntity> 
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // Maps to the User table
            builder.ToTable("user", "identity");

            // Primary key
            builder.HasKey(u => u.Id);

            // Indexes for "normalized" username and email, to allow efficient lookups
            builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

            // A concurrency token for use with the optimistic concurrency checking
            builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(256);
            builder.Property(u => u.NormalizedUserName).IsRequired().HasMaxLength(256);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.NormalizedEmail).IsRequired().HasMaxLength(256);

            // The relationship between User and Role
            builder.HasMany(u => u.Roles)
                   .WithMany(r => r.Users)
                   .UsingEntity<UserRoleEntity>
                   (
                       "user_role", 
                       roleBuilder => roleBuilder.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId),
                       userBuilder => userBuilder.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId),
                       joinBuilder => joinBuilder.ToTable("user_role", "identity")
                   );
        }
    }
}