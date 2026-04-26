using Domain.Comercial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ClienteConfiguration : AuditableEntityConfiguration<Cliente>
    {
        public override void Configure(EntityTypeBuilder<Cliente> builder)
        {
            base.Configure(builder);

            builder.ToTable("Clientes", schema: "comercial");

            builder.Property(c => c.TipoDocumentoId)
                .IsRequired();

            builder.Property(c => c.NumeroDocumento)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Nombres)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ApellidoPaterno)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ApellidoMaterno)
                .HasMaxLength(100);

            builder.Property(c => c.Correo)
                .HasMaxLength(150);

            builder.Property(c => c.Telefono)
                .HasMaxLength(20);

            builder.Property(c => c.Direccion)
                .HasMaxLength(250);

            builder.HasOne(c => c.TipoDocumento)
                .WithMany()
                .HasForeignKey(c => c.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.NombreCompleto)
                .HasComputedColumnSql(
                    "[Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')",
                    stored: true);

            builder.HasIndex(c => new { c.TipoDocumentoId, c.NumeroDocumento })
                .IsUnique();

            builder.HasIndex(c => c.Correo)
                .IsUnique()
                .HasFilter("[Correo] IS NOT NULL");
        }
    }
}
