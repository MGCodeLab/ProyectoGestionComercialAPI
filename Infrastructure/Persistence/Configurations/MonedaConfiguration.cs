using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalogo;

namespace Infrastructure.Persistence.Configurations;

public class MonedaConfiguration : AuditableEntityConfiguration<Moneda>
{
    public override void Configure(EntityTypeBuilder<Moneda> builder)
    {
        base.Configure(builder);

        builder.ToTable("Monedas", schema: "catalogo");

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Simbolo)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(m => m.CodigoISO)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(m => m.EsMonedaBase)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(m => m.CodigoISO)
            .IsUnique();
    }
}
