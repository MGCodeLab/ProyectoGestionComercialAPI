using Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : AuditableEntityConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.ToTable("Usuarios", schema: "seguridad");

            builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(150);
            builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            builder.Property(e => e.LastLogin);

            builder.HasIndex(e => e.Email).IsUnique();
        }
    }
}
