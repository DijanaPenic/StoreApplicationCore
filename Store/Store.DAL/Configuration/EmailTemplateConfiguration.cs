using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities;
using Store.Entities.Identity;

namespace Store.DAL.Configuration
{
    internal class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplateEntity> 
    {
        public void Configure(EntityTypeBuilder<EmailTemplateEntity> builder)
        {
            // Maps to the Book table
            builder.ToTable("email_template");

            // Primary key
            builder.HasKey(et => et.Id);

            // Limit the size of columns to use efficient database types
            builder.Property(et => et.Path).IsRequired().HasMaxLength(250);
            builder.Property(et => et.Name).IsRequired().HasMaxLength(50);

            // Each EmailTemplate must have one Client
            builder.HasOne<ClientEntity>().WithMany().HasForeignKey(et => et.ClientId).IsRequired();
        }
    }
}