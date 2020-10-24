using Microsoft.EntityFrameworkCore;

using Store.Entities;
using Store.Entities.Identity;
using Store.DAL.Configuration;
using Store.DAL.Configuration.Identity;

namespace Store.DAL.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BookEntity> Books { get; set; }

        public DbSet<BookstoreEntity> Bookstores { get; set; }

        public DbSet<UserRoleEntity> UserRoles { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        public DbSet<RoleClaimEntity> RoleClaims { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserClaimEntity> UserClaims { get; set; }

        public DbSet<UserLoginEntity> UserLogins { get; set; }

        public DbSet<UserTokenEntity> UserTokens { get; set; }

        public DbSet<UserRefreshTokenEntity> UserRefreshTokens { get; set; }

        public DbSet<ClientEntity> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity configurations
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());
            builder.ApplyConfiguration(new UserRefreshTokenConfiguration());
            builder.ApplyConfiguration(new ClientConfiguration());

            // Other configurations
            builder.ApplyConfiguration(new BookstoreConfiguration());
            builder.ApplyConfiguration(new BookConfiguration());
        }   
    }
}