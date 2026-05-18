using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class MarcaProductoValidatorService : IMarcaProductoValidatorService
    {
        private readonly AppDbContext _context;

        public MarcaProductoValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
        {
            var existe = await _context.MarcasProducto
                .Where(m => m.Nombre == nombre && (excludeId == null || m.Id != excludeId))
                .AnyAsync();
            return !existe;
        }
    }
}
