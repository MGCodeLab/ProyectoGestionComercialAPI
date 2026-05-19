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

        public async Task<List<CategoriaProducto>> ObtenerTodosAsync()
        {
            return await _context.CategoriasProducto
                .Where(x => x.Activo)
                .Include(x => x.Subcategorias)
                .ToListAsync();
        }

        public async Task<CategoriaProducto?> ObtenerPorIdAsync(int id, bool tracking = false)
        {
            var query = _context.CategoriasProducto.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CategoriaProducto>> ObtenerRaicesAsync()
        {
            return await _context.CategoriasProducto
                .Where(x => x.Activo && x.CategoriaPadreId == null)
                .Include(x => x.Subcategorias)
                .ToListAsync();
        }

        public async Task Crear(CategoriaProducto categoria)
        {
            _context.CategoriasProducto.Add(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(CategoriaProducto categoria)
        {
            _context.CategoriasProducto.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var categoria = await ObtenerPorIdAsync(id, tracking: true);
            if (categoria != null)
            {
                categoria.Activo = false;
                await Actualizar(categoria);
            }
        }
    }
}
