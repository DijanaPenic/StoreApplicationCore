using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLoginEntity> 
    {
        public void Configure(EntityTypeBuilder<UserLoginEntity> builder)
        {
            // Maps to the UserLogin table
            builder.ToTable("user_login", "identity");

            // Composite primary key consisting of the LoginProvider and the key to use with that provider
            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(ul => ul.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(ul => ul.ProviderKey).IsRequired().HasMaxLength(128);
            builder.Property(ul => ul.Token).IsRequired(false).HasMaxLength(300);

            // The relationship between User and UserLogin
            builder.HasOne(ul => ul.User).WithMany(u => u.Logins).HasForeignKey(uc => uc.UserId).IsRequired();
        }
    }
}