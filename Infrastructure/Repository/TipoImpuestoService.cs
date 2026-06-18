using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TipoImpuestoService : ITipoImpuestoService
    {
        private readonly AppDbContext _context;

        public TipoImpuestoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoImpuesto>> ObtenerTodos(CancellationToken token)
            => await _context.TiposImpuesto.ToListAsync(token);

        public async Task<TipoImpuesto?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
            => isAsTracking
                ? await _context.TiposImpuesto.FirstOrDefaultAsync(x => x.Id == id, token)
                : await _context.TiposImpuesto.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task<int> Crear(TipoImpuesto entity, CancellationToken token)
        {
            _context.TiposImpuesto.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);

        public async Task Eliminar(TipoImpuesto entity, CancellationToken token)
        {
            _context.TiposImpuesto.Remove(entity);
            await _context.SaveChangesAsync(token);
        }
    }
}
