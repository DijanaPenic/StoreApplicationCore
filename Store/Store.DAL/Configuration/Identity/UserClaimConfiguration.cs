using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaimEntity> 
    {
        public void Configure(EntityTypeBuilder<UserClaimEntity> builder)
        {
            // Maps to the UserClaim table
            builder.ToTable("user_claim", "identity");

            // Primary key
            builder.HasKey(uc => uc.Id);
        }
    }
}