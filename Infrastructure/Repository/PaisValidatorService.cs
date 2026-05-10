using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class PaisValidatorService : IPaisValidatorService
{
    private readonly AppDbContext _context;

    public PaisValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken)
    {
        var exists = await _context.Paises
            .AnyAsync(p => p.Codigo == codigo.ToUpper(), cancellationToken);
        return !exists;
    }

    public async Task<bool> IsCodigoUniqueExcept(int paisId, string codigo, CancellationToken cancellationToken)
    {
        var exists = await _context.Paises
            .AnyAsync(p => p.Id != paisId && p.Codigo == codigo.ToUpper(), cancellationToken);
        return !exists;
    }
}
