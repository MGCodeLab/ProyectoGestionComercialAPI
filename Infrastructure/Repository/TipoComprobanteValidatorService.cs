using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TipoComprobanteValidatorService
    {
        private readonly AppDbContext _context;

        public TipoComprobanteValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CodigoUnicoAsync(string codigo, int? excludeId = null)
        {
            var existe = await _context.TiposComprobante
                .Where(t => t.Codigo == codigo && (excludeId == null || t.Id != excludeId))
                .AnyAsync();
            return !existe;
        }
    }
}
