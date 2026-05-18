using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
    {
        public void Configure(EntityTypeBuilder<CategoriaProducto> builder)
        {
            builder.ToTable("CategoriasProducto", "catalogo");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Descripcion)
                .HasMaxLength(500);

            builder.Property(c => c.CategoriaPadreId)
                .IsRequired(false);

            builder.Property(c => c.PublicId)
                .IsRequired();

            builder.Property(c => c.Activo)
                .HasDefaultValue(true);

            builder.Property(c => c.FechaRegistro)
                .IsRequired();

            builder.Property(c => c.FechaActualizacion)
                .IsRequired(false);

            // Self-referencing foreign key
            builder.HasOne(c => c.CategoriaPadre)
                .WithMany(c => c.Subcategorias)
                .HasForeignKey(c => c.CategoriaPadreId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indices
            builder.HasIndex(c => c.CategoriaPadreId);
            builder.HasIndex(c => c.Activo);
            builder.HasIndex(c => c.Nombre);
        }
    }
}
