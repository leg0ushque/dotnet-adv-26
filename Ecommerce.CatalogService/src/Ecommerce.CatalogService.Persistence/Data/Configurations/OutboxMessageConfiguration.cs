using Ecommerce.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.CatalogService.Persistence.Data.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Type)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(o => o.Payload)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.ProcessedAt);

            builder.Property(o => o.IsProcessed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(o => o.RetryCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(o => o.Error)
                .HasMaxLength(2000);

            builder.HasIndex(o => o.IsProcessed);
            builder.HasIndex(o => o.CreatedAt);
        }
    }
}
