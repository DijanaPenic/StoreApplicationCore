using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity>  
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            builder.ToTable("UserRole");

            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(ur => ur.RoleId);

            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.Roles)
                   .HasForeignKey(ur => ur.UserId);
        }
    }
}