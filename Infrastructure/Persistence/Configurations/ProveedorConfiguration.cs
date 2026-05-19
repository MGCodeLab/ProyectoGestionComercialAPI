using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Comercial;

namespace Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : AuditableEntityConfiguration<Proveedor>
{
    public override void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        base.Configure(builder);

        builder.ToTable("Proveedores", schema: "comercial");

        builder.Property(p => p.TipoDocumentoId)
            .IsRequired();

        builder.Property(p => p.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.RazonSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.NombreComercial)
            .HasMaxLength(150);

        builder.Property(p => p.PaisId)
            .IsRequired();

        builder.Property(p => p.Correo)
            .HasMaxLength(150);

        builder.Property(p => p.Telefono)
            .HasMaxLength(20);

        builder.Property(p => p.Direccion)
            .HasMaxLength(300);

        builder.HasOne(p => p.TipoDocumento)
            .WithMany()
            .HasForeignKey(p => p.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Pais)
            .WithMany()
            .HasForeignKey(p => p.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.TipoDocumentoId, p.NumeroDocumento })
            .IsUnique()
            .HasName("UX_Proveedores_TipoDocumento_Numero");

        builder.HasIndex(p => p.Correo, "IX_Proveedores_Correo")
            .IsUnique()
            .HasFilter($"[Correo] IS NOT NULL");

        builder.HasIndex(p => p.PaisId);
        builder.HasIndex(p => p.TipoDocumentoId);
    }
}
