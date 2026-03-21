using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos", schema: "catalogo");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500);

            builder.Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Activo)
                .HasDefaultValue(true);
        }
    }
}
