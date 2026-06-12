using Domain.Catalogo;
using Domain.Organizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SerieDocumentoConfiguration : IEntityTypeConfiguration<SerieDocumento>
    {
        public void Configure(EntityTypeBuilder<SerieDocumento> builder)
        {
            builder.ToTable("SeriesDocumento", "catalogo");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TipoComprobanteId)
                .IsRequired();

            builder.Property(s => s.SucursalId)
                .IsRequired();

            builder.Property(s => s.Serie)
                .IsRequired()
                .HasMaxLength(4);

            builder.Property(s => s.NumeroActual)
                .HasDefaultValue(0);

            builder.Property(s => s.NumeroMaximo);

            builder.Property(s => s.PublicId)
                .IsRequired();

            builder.Property(s => s.Activo)
                .HasDefaultValue(true);

            builder.Property(s => s.FechaRegistro)
                .IsRequired();

            builder.Property(s => s.FechaActualizacion)
                .IsRequired(false);

            builder.HasIndex(s => new { s.TipoComprobanteId, s.SucursalId, s.Serie })
                .IsUnique(true);

            builder.HasIndex(s => s.TipoComprobanteId);
            builder.HasIndex(s => s.SucursalId);
            builder.HasIndex(s => s.Activo);

            builder.HasOne(s => s.TipoComprobante)
                .WithMany(t => t.SeriesDocumento)
                .HasForeignKey(s => s.TipoComprobanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Sucursal)
                .WithMany()
                .HasForeignKey(s => s.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
