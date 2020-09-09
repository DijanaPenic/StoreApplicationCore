using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>  
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("User");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.PasswordHash)
                   .HasColumnType("varchar")
                   .IsRequired(false);

            builder.Property(u => u.SecurityStamp)
                   .HasColumnType("varchar")
                   .IsRequired(false);

            builder.Property(u => u.UserName)
                   .HasColumnType("varchar")
                   .HasMaxLength(256)
                   .IsRequired();

            builder.HasMany(u => u.Claims)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId);

            builder.HasMany(u => u.Logins)
                   .WithOne(el => el.User)
                   .HasForeignKey(el => el.UserId);
        }
    }
}