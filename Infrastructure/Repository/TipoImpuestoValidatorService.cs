using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TipoImpuestoValidatorService
    {
        private readonly AppDbContext _context;

        public TipoImpuestoValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CodigoUnicoAsync(string codigo, int? excludeId = null)
        {
            var existe = await _context.TiposImpuesto
                .Where(t => t.Codigo == codigo && (excludeId == null || t.Id != excludeId))
                .AnyAsync();
            return !existe;
        }
    }
}
