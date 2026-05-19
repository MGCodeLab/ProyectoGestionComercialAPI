using Application.Interfaces;
using Domain.Organizacion;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SucursalService : ISucursalService
    {
        private readonly AppDbContext _context;

        public SucursalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sucursal?> ObtenerPorId(int id, bool tracking = false)
            => tracking
                ? await _context.Sucursales.FirstOrDefaultAsync(x => x.Id == id)
                : await _context.Sucursales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Sucursal>> ObtenerTodos()
            => await _context.Sucursales.ToListAsync();

        public async Task<int> Crear(Sucursal sucursal)
        {
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();
            return sucursal.Id;
        }

        public async Task Actualizar(Sucursal sucursal)
        {
            sucursal.FechaActualizacion = DateTime.UtcNow;
            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal != null)
            {
                _context.Sucursales.Remove(sucursal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
