using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodos(CancellationToken token)
            => await _context.Productos.ToListAsync(token);

        public async Task<Producto?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) 
            => (isAsTracking) ?
                await _context.Productos.FirstOrDefaultAsync(x => x.Id == id, token) :
                await _context.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task Crear(Producto producto, CancellationToken token)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync(token);
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);
        
        public async Task Eliminar(Producto producto, CancellationToken token)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync(token);
        }
    }
}
