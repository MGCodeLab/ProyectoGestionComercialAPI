using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalogo;

namespace Infrastructure.Persistence.Configurations;

public class ListaPrecioConfiguration : AuditableEntityConfiguration<ListaPrecio>
{
    public override void Configure(EntityTypeBuilder<ListaPrecio> builder)
    {
        base.Configure(builder);

        builder.ToTable("ListasPrecios", schema: "catalogo");

        builder.Property(l => l.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(l => l.MonedaId)
            .IsRequired();

        builder.Property(l => l.Descripcion)
            .HasMaxLength(500);

        builder.Property(l => l.EsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(l => l.Moneda)
            .WithMany()
            .HasForeignKey(l => l.MonedaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.MonedaId);
        builder.HasIndex(l => l.EsDefault);
    }
}
