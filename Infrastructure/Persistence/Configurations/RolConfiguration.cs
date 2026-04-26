using Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RolConfiguration : AuditableEntityConfiguration<Rol>
    {
        public override void Configure(EntityTypeBuilder<Rol> builder)
        {
            base.Configure(builder);

            builder.ToTable("Roles", schema: "seguridad");

            builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Descripcion).HasMaxLength(200);

            builder.HasIndex(e => e.Nombre).IsUnique();
        }
    }
}
