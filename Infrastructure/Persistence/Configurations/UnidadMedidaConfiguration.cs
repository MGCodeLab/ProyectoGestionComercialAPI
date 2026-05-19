using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalogo;

namespace Infrastructure.Persistence.Configurations;

public class UnidadMedidaConfiguration : AuditableEntityConfiguration<UnidadMedida>
{
    public override void Configure(EntityTypeBuilder<UnidadMedida> builder)
    {
        base.Configure(builder);

        builder.ToTable("UnidadesMedida", schema: "catalogo");

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Simbolo)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.Codigo)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(u => u.Codigo)
            .IsUnique();

        builder.HasIndex(u => u.Simbolo);
    }
}
