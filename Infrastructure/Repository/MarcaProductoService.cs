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

        public async Task<List<MarcaProducto>> ObtenerTodosAsync(CancellationToken cancellationToken)
        {
            return await _context.MarcasProducto.Where(x => x.Activo).ToListAsync(cancellationToken);
        }

        public async Task<MarcaProducto?> ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken)
        {
            var query = _context.MarcasProducto.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task Crear(MarcaProducto marca, CancellationToken cancellationToken)
        {
            _context.MarcasProducto.Add(marca);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Actualizar(MarcaProducto marca, CancellationToken cancellationToken)
        {
            _context.MarcasProducto.Update(marca);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Eliminar(int id, CancellationToken cancellationToken)
        {
            var marca = await ObtenerPorIdAsync(id, tracking: true, cancellationToken);
            if (marca != null)
            {
                marca.Activo = false;
                await Actualizar(marca, cancellationToken);
            }
        }
    }
}
