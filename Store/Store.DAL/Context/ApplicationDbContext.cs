using System;
using System.Linq;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Store.Entities;
using Store.Entities.Identity;
using Store.Common.Helpers;
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

        public DbSet<EmailTemplateEntity> EmailTemplates { get; set; }

        public DbSet<UserRoleEntity> UserRoles { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        public DbSet<RoleClaimEntity> RoleClaims { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserClaimEntity> UserClaims { get; set; }

        public DbSet<UserLoginEntity> UserLogins { get; set; }

        public DbSet<UserTokenEntity> UserTokens { get; set; }

        public DbSet<UserRefreshTokenEntity> UserRefreshTokens { get; set; }

        public DbSet<ClientEntity> Clients { get; set; }

        public IDbConnection Connection => Database.GetDbConnection();  // multiple calls will return the same connection

        public IDbTransaction Transaction => Database.CurrentTransaction?.GetDbTransaction();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Shared Type Entites
            builder.SharedTypeEntity<UserRoleEntity>("user_role");

            // Identity configurations
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());
            builder.ApplyConfiguration(new UserRefreshTokenConfiguration());
            builder.ApplyConfiguration(new ClientConfiguration());

            // Other configurations
            builder.ApplyConfiguration(new BookstoreConfiguration());
            builder.ApplyConfiguration(new BookConfiguration());
            builder.ApplyConfiguration(new EmailTemplateConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            DateTime utcNow = DateTime.UtcNow;

            foreach (EntityEntry entry in entries)
            {
                object entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (entity is IDBBaseEntity) (entity as IDBBaseEntity).DateCreatedUtc = utcNow;
                    if (entry.Metadata.FindProperty("Id") != null) entry.Property("Id").CurrentValue = GuidHelper.NewSequentialGuid();
                }

                if (entity is IDBChangable) (entity as IDBChangable).DateUpdatedUtc = utcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}