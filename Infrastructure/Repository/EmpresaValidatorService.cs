using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class EmpresaValidatorService : IEmpresaValidatorService
    {
        private readonly AppDbContext _context;

        public EmpresaValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsNumeroDocumentoUnique(string numeroDocumento, CancellationToken ct)
        {
            var exists = await _context.Empresas
                .AnyAsync(x => x.NumeroDocumento == numeroDocumento, ct);
            return !exists;
        }
    }
}
