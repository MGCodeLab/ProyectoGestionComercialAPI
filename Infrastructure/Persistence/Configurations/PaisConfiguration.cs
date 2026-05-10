using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalogo;

namespace Infrastructure.Persistence.Configurations;

public class PaisConfiguration : AuditableEntityConfiguration<Pais>
{
    public override void Configure(EntityTypeBuilder<Pais> builder)
    {
        base.Configure(builder);

        builder.ToTable("Paises", schema: "catalogo");

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(p => p.CodigoMoneda)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasIndex(p => p.Codigo)
            .IsUnique();

        builder.HasIndex(p => p.CodigoMoneda);
    }
}
