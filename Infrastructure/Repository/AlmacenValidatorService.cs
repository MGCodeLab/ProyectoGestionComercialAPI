using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AlmacenValidatorService : IAlmacenValidatorService
    {
        private readonly AppDbContext _context;

        public AlmacenValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCodigoUnique(string codigo, CancellationToken ct)
        {
            var exists = await _context.Almacenes
                .AnyAsync(x => x.Codigo == codigo, ct);
            return !exists;
        }
    }
}
