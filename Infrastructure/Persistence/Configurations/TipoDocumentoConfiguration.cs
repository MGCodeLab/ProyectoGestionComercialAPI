using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TipoDocumentoConfiguration : AuditableEntityConfiguration<TipoDocumento>
    {
        public override void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            base.Configure(builder);

            builder.ToTable("TipoDocumentos", schema: "catalogo");

            builder.Property(e => e.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Descripcion)
                .HasMaxLength(250);

            builder.HasIndex(e => e.Codigo)
                .IsUnique();
        }
    }
}
