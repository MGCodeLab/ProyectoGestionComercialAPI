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

            // Sprint 4: Foreign keys for enriched product
            builder.HasOne(p => p.UnidadMedida)
                .WithMany()
                .HasForeignKey(p => p.UnidadMedidaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.CategoriaProducto)
                .WithMany()
                .HasForeignKey(p => p.CategoriaProductoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.MarcaProducto)
                .WithMany()
                .HasForeignKey(p => p.MarcaProductoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
