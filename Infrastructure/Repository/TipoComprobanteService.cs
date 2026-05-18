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

        public async Task<List<TipoComprobante>> ObtenerTodosAsync()
        {
            return await _context.TiposComprobante.Where(x => x.Activo).ToListAsync();
        }

        public async Task<TipoComprobante> ObtenerPorIdAsync(int id)
        {
            return await _context.TiposComprobante.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Crear(TipoComprobante tipoComprobante)
        {
            _context.TiposComprobante.Add(tipoComprobante);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(TipoComprobante tipoComprobante)
        {
            _context.TiposComprobante.Update(tipoComprobante);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var tipoComprobante = await ObtenerPorIdAsync(id);
            if (tipoComprobante != null)
            {
                tipoComprobante.Activo = false;
                await Actualizar(tipoComprobante);
            }
        }
    }
}
