using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class MonedaService : IMonedaService
{
    private readonly AppDbContext _context;
    public MonedaService(AppDbContext context) => _context = context;
    public async Task<List<Moneda>> ObtenerTodos(CancellationToken token) => await _context.Monedas.AsNoTracking().ToListAsync(token);
    public async Task<Moneda?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) => await _context.Monedas.FirstOrDefaultAsync(m => m.Id == id, token);
    public async Task<int> Crear(Moneda entity, CancellationToken token) { _context.Monedas.Add(entity); await _context.SaveChangesAsync(token); return entity.Id; }
    public async Task Actualizar(CancellationToken token) => await _context.SaveChangesAsync(token);
    public async Task Eliminar(Moneda entity, CancellationToken token) { _context.Monedas.Remove(entity); await _context.SaveChangesAsync(token); }

    public async Task<bool> TieneDependencias(Moneda moneda, CancellationToken token)
    {
        // Verificar si la moneda está vinculada en Paises (por CodigoMoneda)
        var existeEnPaises = await _context.Paises
            .AsNoTracking()
            .AnyAsync(p => p.CodigoMoneda == moneda.CodigoISO, token);

        if (existeEnPaises)
            return true;

        // Verificar si la moneda es moneda base en alguna Empresa
        var existeEnEmpresas = await _context.Empresas
            .AsNoTracking()
            .AnyAsync(e => e.MonedaBaseId == moneda.Id, token);

        return existeEnEmpresas;
    }
}
