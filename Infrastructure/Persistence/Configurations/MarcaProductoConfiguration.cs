using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MarcaProductoConfiguration : IEntityTypeConfiguration<MarcaProducto>
    {
        public void Configure(EntityTypeBuilder<MarcaProducto> builder)
        {
            builder.ToTable("MarcasProducto", "catalogo");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(m => m.Descripcion)
                .HasMaxLength(500);

            builder.Property(m => m.LogoUrl)
                .HasMaxLength(500);

            builder.Property(m => m.PublicId)
                .IsRequired();

            builder.Property(m => m.Activo)
                .HasDefaultValue(true);

            builder.Property(m => m.FechaRegistro)
                .IsRequired();

            builder.Property(m => m.FechaActualizacion)
                .IsRequired(false);

            // Indices
            builder.HasIndex(m => m.Nombre);
            builder.HasIndex(m => m.Activo);
        }
    }
}
