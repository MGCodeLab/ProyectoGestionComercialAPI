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

        public async Task<List<TipoImpuesto>> ObtenerTodosAsync()
        {
            return await _context.TiposImpuesto.Where(x => x.Activo).ToListAsync();
        }

        public async Task<TipoImpuesto> ObtenerPorIdAsync(int id)
        {
            return await _context.TiposImpuesto.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Crear(TipoImpuesto tipoImpuesto)
        {
            _context.TiposImpuesto.Add(tipoImpuesto);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(TipoImpuesto tipoImpuesto)
        {
            _context.TiposImpuesto.Update(tipoImpuesto);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var tipoImpuesto = await ObtenerPorIdAsync(id);
            if (tipoImpuesto != null)
            {
                tipoImpuesto.Activo = false;
                await Actualizar(tipoImpuesto);
            }
        }
    }
}
