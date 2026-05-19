using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TipoComprobanteConfiguration : IEntityTypeConfiguration<TipoComprobante>
    {
        public void Configure(EntityTypeBuilder<TipoComprobante> builder)
        {
            builder.ToTable("TiposComprobante", "catalogo");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Codigo)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.AfectaInventario)
                .HasDefaultValue(true);

            builder.Property(t => t.AfectaContable)
                .HasDefaultValue(true);

            builder.Property(t => t.PublicId)
                .IsRequired();

            builder.Property(t => t.Activo)
                .HasDefaultValue(true);

            builder.Property(t => t.FechaRegistro)
                .IsRequired();

            builder.Property(t => t.FechaActualizacion)
                .IsRequired();

            builder.HasIndex(t => t.Codigo)
                .IsUnique();

            builder.HasIndex(t => t.Activo);

            builder.HasMany(t => t.SeriesDocumento)
                .WithOne(s => s.TipoComprobante)
                .HasForeignKey(s => s.TipoComprobanteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
