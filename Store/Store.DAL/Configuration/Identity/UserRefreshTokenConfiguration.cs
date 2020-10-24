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
            builder.ToTable("user_refresh_token");

            // Primary key
            builder.HasKey(rt => rt.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(rt => rt.Value).IsRequired().HasMaxLength(256);

            // Each RefreshToken must have one User
            builder.HasOne<UserEntity>().WithMany().HasForeignKey(rt => rt.UserId).IsRequired();

            // Each RefreshToken must have one Client
            builder.HasOne<ClientEntity>().WithMany().HasForeignKey(rt => rt.ClientId).IsRequired(); 
        }
    }
}