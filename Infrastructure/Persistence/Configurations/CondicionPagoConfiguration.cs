using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalogo;

namespace Infrastructure.Persistence.Configurations;

public class CondicionPagoConfiguration : AuditableEntityConfiguration<CondicionPago>
{
    public override void Configure(EntityTypeBuilder<CondicionPago> builder)
    {
        base.Configure(builder);

        builder.ToTable("CondicionesPago", schema: "catalogo");

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DiasCredito)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Nombre)
            .IsUnique();
    }
}
