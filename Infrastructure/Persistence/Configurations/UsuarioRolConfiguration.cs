using Domain.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
    {
        public void Configure(EntityTypeBuilder<UsuarioRol> builder)
        {
            builder.ToTable("UsuarioRoles", schema: "seguridad");

            builder.HasKey(e => new { e.UsuarioId, e.RolId });

            builder.HasOne(e => e.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
