using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class CategoriaProductoService : ICategoriaProductoService
    {
        private readonly AppDbContext _context;

        public CategoriaProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaProducto>> ObtenerTodosAsync(CancellationToken cancellationToken)
        {
            return await _context.CategoriasProducto
                .Where(x => x.Activo)
                .Include(x => x.Subcategorias)
                .ToListAsync(cancellationToken);
        }

        public async Task<CategoriaProducto?> ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken)
        {
            var query = _context.CategoriasProducto.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<CategoriaProducto>> ObtenerRaicesAsync(CancellationToken cancellationToken)
        {
            return await _context.CategoriasProducto
                .Where(x => x.Activo && x.CategoriaPadreId == null)
                .Include(x => x.Subcategorias)
                .ToListAsync(cancellationToken);
        }

        public async Task Crear(CategoriaProducto categoria, CancellationToken cancellationToken)
        {
            _context.CategoriasProducto.Add(categoria);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Actualizar(CategoriaProducto categoria, CancellationToken cancellationToken)
        {
            _context.CategoriasProducto.Update(categoria);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Eliminar(int id, CancellationToken cancellationToken)
        {
            var categoria = await ObtenerPorIdAsync(id, tracking: true, cancellationToken);
            if (categoria != null)
            {
                categoria.Activo = false;
                await Actualizar(categoria, cancellationToken);
            }
        }
    }
}
