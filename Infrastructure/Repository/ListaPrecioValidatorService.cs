using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ListaPrecioValidatorService : IListaPrecioValidatorService
{
    private readonly AppDbContext _context;

    public ListaPrecioValidatorService(AppDbContext context) => _context = context;

    public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
    {
        var existe = await _context.ListasPrecios
            .AnyAsync(x => x.Nombre == nombre && (excludeId == null || x.Id != excludeId));
        return !existe;
    }

    public async Task<bool> ExisteDefaultAsync(int? excludeId = null)
    {
        var existe = await _context.ListasPrecios
            .AnyAsync(x => x.EsDefault && x.Activo && (excludeId == null || x.Id != excludeId));
        return existe;
    }

    public async Task<bool> MonedaExisteAsync(int monedaId)
    {
        return await _context.Monedas
            .AnyAsync(m => m.Id == monedaId && m.Activo);
    }
}
