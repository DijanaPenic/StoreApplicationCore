using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class ClaimConfiguration : IEntityTypeConfiguration<ClaimEntity>
    {
        public void Configure(EntityTypeBuilder<ClaimEntity> builder)
        {
            builder.ToTable("Claim");

            builder.Property(c => c.Id)
                   .HasColumnType("int")
                   .ValueGeneratedOnAdd()
                   .IsRequired();

            builder.Property(c => c.UserId)
                   .HasColumnType("uuid")
                   .IsRequired();

            builder.Property(c => c.ClaimType)
                   .HasMaxLength(150)
                   .HasColumnType("varchar")
                   .IsRequired(false);

            builder.Property(c => c.ClaimValue)
                   .HasColumnType("varchar")
                   .IsRequired(false);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Claims)
                   .HasForeignKey(c => c.UserId);
        }
    }
}