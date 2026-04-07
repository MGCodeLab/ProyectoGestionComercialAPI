using Domain.Catalogo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class TipoDocumentoConfiguration : IEntityTypeConfiguration<TipoDocumento>
    {
        public void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            builder.ToTable("TipoDocumento", schema: "catalogo");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Descripcion)
                .HasMaxLength(250);

            builder.Property(e => e.Activo)
                .IsRequired();
        }
    }
}
