using Microsoft.EntityFrameworkCore;
using Domain.Catalogo;
using Domain.Comercial;
using Domain.Common;
using Domain.Seguridad;
using Domain.Configuracion;
using Domain.Organizacion;

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

            base.OnModelCreating(modelBuilder);
        }

        // Catalogo
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<TipoImpuesto> TiposImpuesto { get; set; }
        public DbSet<TipoComprobante> TiposComprobante { get; set; }
        public DbSet<SerieDocumento> SeriesDocumento { get; set; }

        // Sprint 4: Catalogo Enriquecido
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<MarcaProducto> MarcasProducto { get; set; }

        // Sprint 5: Catalogo Comercial
        public DbSet<CondicionPago> CondicionesPago { get; set; }
        public DbSet<ListaPrecio> ListasPrecios { get; set; }

        // Comercial
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        // Organizacion
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }

        // Seguridad
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }

        // Configuracion
        public DbSet<ModuloSistema> ModulosSistema { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }
    }
}
