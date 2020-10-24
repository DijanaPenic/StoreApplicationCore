using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserTokenConfiguration : IEntityTypeConfiguration<UserTokenEntity> 
    {
        public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
        {
            // Maps to the UserToken table
            builder.ToTable("user_token");

            // Composite primary key consisting of the UserId, LoginProvider and Name
            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(ut => ut.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(ut => ut.Name).IsRequired().HasMaxLength(128);
        }
    }
}