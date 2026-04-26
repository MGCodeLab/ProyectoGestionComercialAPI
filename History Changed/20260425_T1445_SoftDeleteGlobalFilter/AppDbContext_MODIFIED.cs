using Microsoft.EntityFrameworkCore;
using Domain.Catalogo;
using Domain.Comercial;
using Domain.Common;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Global soft delete filter: exclude all inactive (Activo=false) records by default
            ApplySoftDeleteFilters(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
        {
            // Apply soft delete filter to all AuditableEntity types
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entity.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entity.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, "Activo");
                    var constant = System.Linq.Expressions.Expression.Constant(true);
                    var equalExpression = System.Linq.Expressions.Expression.Equal(property, constant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(equalExpression, parameter);

                    entity.SetQueryFilter(lambda);
                }
            }
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
    }
}
