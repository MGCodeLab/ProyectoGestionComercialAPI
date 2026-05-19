using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class CondicionPagoValidatorService : ICondicionPagoValidatorService
{
    private readonly AppDbContext _context;

    public CondicionPagoValidatorService(AppDbContext context) => _context = context;

    public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
    {
        var existe = await _context.CondicionesPago
            .AnyAsync(x => x.Nombre == nombre && (excludeId == null || x.Id != excludeId));
        return !existe;
    }
}
