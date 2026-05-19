using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class ModuloSistemaValidatorService : IModuloSistemaValidatorService
{
    private readonly AppDbContext _context;

    public ModuloSistemaValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken)
    {
        return !await _context.ModulosSistema
            .AnyAsync(m => m.Codigo == codigo.ToUpper(), cancellationToken);
    }

    public async Task<bool> IsCodigoUniqueExcept(int moduloId, string codigo, CancellationToken cancellationToken)
    {
        return !await _context.ModulosSistema
            .AnyAsync(m => m.Codigo == codigo.ToUpper() && m.Id != moduloId, cancellationToken);
    }
}
