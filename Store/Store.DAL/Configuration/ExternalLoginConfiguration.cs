using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLoginEntity>
    {
        public void Configure(EntityTypeBuilder<ExternalLoginEntity> builder)
        {
            builder.ToTable("ExternalLogin");

            builder.HasKey(el => new { el.LoginProvider, el.ProviderKey, el.UserId });

            builder.Property(el => el.LoginProvider)
                   .HasColumnType("varchar")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(el => el.ProviderKey)
                   .HasColumnType("varchar")
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(el => el.UserId)
                   .HasColumnType("uuid")
                   .IsRequired();
             
            builder.HasOne(el => el.User)
                   .WithMany(u => u.Logins)
                   .HasForeignKey(el => el.UserId);
        }
    }
}