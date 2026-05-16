using Domain.Organizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AlmacenConfiguration : AuditableEntityConfiguration<Almacen>
    {
        public override void Configure(EntityTypeBuilder<Almacen> builder)
        {
            base.Configure(builder);

            builder.ToTable("Almacenes", schema: "organizacion");

            builder.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(a => a.SucursalId)
                .IsRequired();

            builder.Property(a => a.Descripcion)
                .HasMaxLength(500);

            builder.Property(a => a.EsPrincipal)
                .HasDefaultValue(false);

            builder.HasIndex(a => a.Codigo)
                .IsUnique();

            builder.HasOne(a => a.Sucursal)
                .WithMany()
                .HasForeignKey(a => a.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
