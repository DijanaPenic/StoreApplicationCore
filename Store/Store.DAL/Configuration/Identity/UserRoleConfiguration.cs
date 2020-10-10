using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity> 
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            // Primary key
            builder.HasKey(r => new { r.UserId, r.RoleId });

            // Maps to the UserRole table
            builder.ToTable("user_role");
        }
    }
}