using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class RoleConfiguration : IEntityTypeConfiguration<RoleEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Role");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .HasColumnType("varchar")
                   .HasMaxLength(256)
                   .IsRequired();
        }
    }
}