using Domain.Organizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmpresaConfiguration : AuditableEntityConfiguration<Empresa>
    {
        public override void Configure(EntityTypeBuilder<Empresa> builder)
        {
            base.Configure(builder);

            builder.ToTable("Empresas", schema: "organizacion");

            builder.Property(e => e.RazonSocial)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.NombreComercial)
                .HasMaxLength(200);

            builder.Property(e => e.NumeroDocumento)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.TipoDocumentoId)
                .IsRequired();

            builder.Property(e => e.PaisId)
                .IsRequired();

            builder.Property(e => e.MonedaBaseId)
                .IsRequired();

            builder.Property(e => e.DireccionFiscal)
                .HasMaxLength(300);

            builder.Property(e => e.Telefono)
                .HasMaxLength(20);

            builder.Property(e => e.Correo)
                .HasMaxLength(150);

            builder.Property(e => e.LogoUrl)
                .HasMaxLength(500);

            builder.HasIndex(e => e.NumeroDocumento)
                .IsUnique();

            builder.HasOne(e => e.TipoDocumento)
                .WithMany()
                .HasForeignKey(e => e.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Pais)
                .WithMany()
                .HasForeignKey(e => e.PaisId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.MonedaBase)
                .WithMany()
                .HasForeignKey(e => e.MonedaBaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
