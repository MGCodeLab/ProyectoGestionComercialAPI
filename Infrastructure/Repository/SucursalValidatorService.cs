using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SucursalValidatorService : ISucursalValidatorService
    {
        private readonly AppDbContext _context;

        public SucursalValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCodigoUnique(string codigo, CancellationToken ct)
        {
            var exists = await _context.Sucursales
                .AnyAsync(x => x.Codigo == codigo, ct);
            return !exists;
        }
    }
}
