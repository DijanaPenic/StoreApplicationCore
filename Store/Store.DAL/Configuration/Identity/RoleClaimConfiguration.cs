using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaimEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
        {
            // Primary key
            builder.HasKey(rc => rc.Id);

            // Maps to the RoleClaim table
            builder.ToTable("role_claim");
        }
    }
}