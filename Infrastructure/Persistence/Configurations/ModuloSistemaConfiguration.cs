using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Configuracion;

namespace Infrastructure.Persistence.Configurations;

public class ModuloSistemaConfiguration : AuditableEntityConfiguration<ModuloSistema>
{
    public override void Configure(EntityTypeBuilder<ModuloSistema> builder)
    {
        base.Configure(builder);

        builder.ToTable("ModulosSistema", schema: "configuracion");

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Descripcion)
            .HasMaxLength(500);

        builder.HasIndex(m => m.Codigo)
            .IsUnique();
    }
}
