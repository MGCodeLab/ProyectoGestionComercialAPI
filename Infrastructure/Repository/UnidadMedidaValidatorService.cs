using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class UnidadMedidaValidatorService : IUnidadMedidaValidatorService
{
    private readonly AppDbContext _context;

    public UnidadMedidaValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken)
    {
        return !await _context.UnidadesMedida
            .AnyAsync(u => u.Codigo == codigo.ToUpper(), cancellationToken);
    }

    public async Task<bool> IsCodigoUniqueExcept(int unidadId, string codigo, CancellationToken cancellationToken)
    {
        return !await _context.UnidadesMedida
            .AnyAsync(u => u.Codigo == codigo.ToUpper() && u.Id != unidadId, cancellationToken);
    }
}
