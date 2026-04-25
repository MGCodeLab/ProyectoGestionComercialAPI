using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductoConfiguration : AuditableEntityConfiguration<Producto>
    {
        public override void Configure(EntityTypeBuilder<Producto> builder)
        {
            base.Configure(builder);

            builder.ToTable("Productos", schema: "catalogo");

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500);

            builder.Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");
        }
    }
}
