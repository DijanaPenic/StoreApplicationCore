using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserTokenConfiguration : IEntityTypeConfiguration<UserTokenEntity> 
    {
        public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
        {
            // Composite primary key consisting of the UserId, LoginProvider and Name
            builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(t => t.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(128);

            // Maps to the UserToken table
            builder.ToTable("user_token");
        }
    }
}