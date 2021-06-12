using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaimEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
        {
            // Maps to the RoleClaim table
            builder.ToTable("role_claim", "identity");

            // Primary key
            builder.HasKey(rc => rc.Id);

            // The relationship between Role and RoleClaim
            builder.HasOne(rc => rc.Role).WithMany(r => r.Claims).HasForeignKey(rc => rc.RoleId).IsRequired();  
        }
    }
}