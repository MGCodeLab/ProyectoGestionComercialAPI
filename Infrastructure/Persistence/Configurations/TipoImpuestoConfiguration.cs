using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TipoImpuestoConfiguration : IEntityTypeConfiguration<TipoImpuesto>
    {
        public void Configure(EntityTypeBuilder<TipoImpuesto> builder)
        {
            builder.ToTable("TiposImpuesto", "catalogo");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(t => t.Porcentaje)
                .HasColumnType("DECIMAL(5,2)")
                .HasDefaultValue(0.00m);

            builder.Property(t => t.EsIncluido)
                .HasDefaultValue(true);

            builder.Property(t => t.PublicId)
                .IsRequired();

            builder.Property(t => t.Activo)
                .HasDefaultValue(true);

            builder.Property(t => t.FechaRegistro)
                .IsRequired();

            builder.Property(t => t.FechaActualizacion)
                .IsRequired(false);

            builder.HasIndex(t => t.Codigo)
                .IsUnique();

            builder.HasIndex(t => t.Activo);
        }
    }
}
