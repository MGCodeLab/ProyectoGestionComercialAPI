using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Base configuration for all AuditableEntity types.
    /// Ensures consistent audit field configuration across all entities.
    /// </summary>
    public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : AuditableEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            // Public identifier - exposed externally instead of internal Id
            builder.Property(e => e.PublicId)
                .IsRequired()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Audit fields - mandatory for all entities
            builder.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.FechaRegistro)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.FechaActualizacion);

            // Index on PublicId for fast lookups by external identifier
            builder.HasIndex(e => e.PublicId)
                .IsUnique();
        }
    }
}
