using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshTokenEntity> 
    {
        public void Configure(EntityTypeBuilder<UserRefreshTokenEntity> builder)
        {
            // Maps to the RefreshToken table
            builder.ToTable("user_refresh_token", "identity");

            // Primary key
            builder.HasKey(urt => new { urt.UserId, urt.ClientId });

            // Limit the size of columns to use efficient database types
            builder.Property(urt => urt.Value).IsRequired().HasMaxLength(256);

            // The relationship between User and UserRefreshToken
            builder.HasOne(urt => urt.User).WithMany(u => u.RefreshTokens).HasForeignKey(urt => urt.UserId).IsRequired();

            // The relationship between Client and UserRefreshToken
            builder.HasOne(urt => urt.Client).WithMany(c => c.RefreshTokens).HasForeignKey(urt => urt.ClientId).IsRequired();
        }
    }
}