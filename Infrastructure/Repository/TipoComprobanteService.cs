using Application.Dtos;
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TipoComprobanteService : ITipoComprobanteService
    {
        private readonly AppDbContext _context;

        public TipoComprobanteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoComprobante>> ObtenerTodos(CancellationToken token)
            => await _context.TiposComprobante.ToListAsync(token);

        public async Task<TipoComprobante?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
            => isAsTracking
                ? await _context.TiposComprobante.FirstOrDefaultAsync(x => x.Id == id, token)
                : await _context.TiposComprobante.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task<int> Crear(TipoComprobante entity, CancellationToken token)
        {
            _context.TiposComprobante.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);

        public async Task Eliminar(TipoComprobante entity, CancellationToken token)
        {
            _context.TiposComprobante.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task<List<ComboDto>> ObtenerCombo(CancellationToken token)
            => await _context.TiposComprobante
                .AsNoTracking()
                .Where(x => x.Activo)
                .Select(x => new ComboDto { Id = x.Id, Nombre = x.Nombre })
                .ToListAsync(token);

        public async Task<bool> TieneDependencias(TipoComprobante entity, CancellationToken cancellationToken)
        {
            var existeSerieDocumento = await _context.SeriesDocumento
                .AsNoTracking()
                .AnyAsync(e => e.TipoComprobanteId == entity.Id, cancellationToken);

            return existeSerieDocumento;
        }
    }
}
