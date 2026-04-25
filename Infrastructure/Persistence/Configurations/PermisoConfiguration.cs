using Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PermisoConfiguration : AuditableEntityConfiguration<Permiso>
    {
        public override void Configure(EntityTypeBuilder<Permiso> builder)
        {
            base.Configure(builder);

            builder.ToTable("Permisos", schema: "seguridad");

            builder.Property(e => e.Recurso).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Accion).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Descripcion).HasMaxLength(200);

            builder.HasIndex(e => new { e.Recurso, e.Accion }).IsUnique();
        }
    }
}
