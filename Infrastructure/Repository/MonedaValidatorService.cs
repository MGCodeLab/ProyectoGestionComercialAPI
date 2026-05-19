using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class MonedaValidatorService : IMonedaValidatorService
{
    private readonly AppDbContext _context;

    public MonedaValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCodigoISOUnique(string codigoISO, CancellationToken cancellationToken)
    {
        var exists = await _context.Monedas
            .AnyAsync(m => m.CodigoISO == codigoISO.ToUpper(), cancellationToken);
        return !exists;
    }

    public async Task<bool> IsCodigoISOUniqueExcept(int monedaId, string codigoISO, CancellationToken cancellationToken)
    {
        var exists = await _context.Monedas
            .AnyAsync(m => m.Id != monedaId && m.CodigoISO == codigoISO.ToUpper(), cancellationToken);
        return !exists;
    }
}
