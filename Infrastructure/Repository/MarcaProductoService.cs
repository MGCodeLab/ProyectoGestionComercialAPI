using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class MarcaProductoService : IMarcaProductoService
    {
        private readonly AppDbContext _context;

        public MarcaProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MarcaProducto>> ObtenerTodosAsync()
        {
            return await _context.MarcasProducto.Where(x => x.Activo).ToListAsync();
        }

        public async Task<MarcaProducto?> ObtenerPorIdAsync(int id, bool tracking = false)
        {
            var query = _context.MarcasProducto.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Crear(MarcaProducto marca)
        {
            _context.MarcasProducto.Add(marca);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(MarcaProducto marca)
        {
            _context.MarcasProducto.Update(marca);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var marca = await ObtenerPorIdAsync(id, tracking: true);
            if (marca != null)
            {
                marca.Activo = false;
                await Actualizar(marca);
            }
        }
    }
}
