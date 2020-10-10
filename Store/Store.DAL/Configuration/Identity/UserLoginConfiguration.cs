using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLoginEntity> 
    {
        public void Configure(EntityTypeBuilder<UserLoginEntity> builder)
        {
            // Composite primary key consisting of the LoginProvider and the key to use
            // with that provider
            builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(l => l.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(l => l.ProviderKey).IsRequired().HasMaxLength(128);

            // Maps to the UserLogin table
            builder.ToTable("user_login");
        }
    }
}