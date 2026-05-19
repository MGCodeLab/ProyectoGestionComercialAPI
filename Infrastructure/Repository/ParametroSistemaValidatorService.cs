using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class ParametroSistemaValidatorService : IParametroSistemaValidatorService
{
    private readonly AppDbContext _context;

    public ParametroSistemaValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsClaveUnique(string clave, CancellationToken cancellationToken)
    {
        return !await _context.ParametrosSistema
            .AnyAsync(p => p.Clave == clave.ToUpper(), cancellationToken);
    }

    public async Task<bool> IsClaveUniqueExcept(int parametroId, string clave, CancellationToken cancellationToken)
    {
        return !await _context.ParametrosSistema
            .AnyAsync(p => p.Clave == clave.ToUpper() && p.Id != parametroId, cancellationToken);
    }
}
