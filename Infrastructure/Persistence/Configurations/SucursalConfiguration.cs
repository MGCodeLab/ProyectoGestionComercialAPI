using Domain.Organizacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SucursalConfiguration : AuditableEntityConfiguration<Sucursal>
    {
        public override void Configure(EntityTypeBuilder<Sucursal> builder)
        {
            base.Configure(builder);

            builder.ToTable("Sucursales", schema: "organizacion");

            builder.Property(s => s.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.EmpresaId)
                .IsRequired();

            builder.Property(s => s.PaisId)
                .IsRequired();

            builder.Property(s => s.Direccion)
                .HasMaxLength(300);

            builder.Property(s => s.Telefono)
                .HasMaxLength(20);

            builder.Property(s => s.EsPrincipal)
                .HasDefaultValue(false);

            builder.HasIndex(s => s.Codigo)
                .IsUnique();

            builder.HasOne(s => s.Empresa)
                .WithMany()
                .HasForeignKey(s => s.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Pais)
                .WithMany()
                .HasForeignKey(s => s.PaisId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
