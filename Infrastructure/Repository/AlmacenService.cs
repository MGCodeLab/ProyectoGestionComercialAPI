using Application.Interfaces;
using Domain.Organizacion;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AlmacenService : IAlmacenService
    {
        private readonly AppDbContext _context;

        public AlmacenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Almacen?> ObtenerPorId(int id, bool tracking = false)
            => tracking
                ? await _context.Almacenes.FirstOrDefaultAsync(x => x.Id == id)
                : await _context.Almacenes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Almacen>> ObtenerTodos()
            => await _context.Almacenes.ToListAsync();

        public async Task<int> Crear(Almacen almacen)
        {
            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();
            return almacen.Id;
        }

        public async Task Actualizar(Almacen almacen)
        {
            almacen.FechaActualizacion = DateTime.UtcNow;
            _context.Almacenes.Update(almacen);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);
            if (almacen != null)
            {
                _context.Almacenes.Remove(almacen);
                await _context.SaveChangesAsync();
            }
        }
    }
}
