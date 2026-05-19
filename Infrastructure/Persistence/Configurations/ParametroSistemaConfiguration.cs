using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Configuracion;

namespace Infrastructure.Persistence.Configurations;

public class ParametroSistemaConfiguration : AuditableEntityConfiguration<ParametroSistema>
{
    public override void Configure(EntityTypeBuilder<ParametroSistema> builder)
    {
        base.Configure(builder);

        builder.ToTable("ParametrosSistema", schema: "configuracion");

        builder.Property(p => p.Clave)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.TipoDato)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("STRING");

        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);

        builder.HasIndex(p => p.Clave)
            .IsUnique();
    }
}
