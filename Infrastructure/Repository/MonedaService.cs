using Microsoft.EntityFrameworkCore;
using Application.Dtos;
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class MonedaService : IMonedaService
{
    private readonly AppDbContext _context;
    public MonedaService(AppDbContext context) => _context = context;
    public async Task<List<Moneda>> ObtenerTodos(CancellationToken token) => await _context.Monedas.AsNoTracking().ToListAsync(token);
    public async Task<Moneda?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
        => isAsTracking
            ? await _context.Monedas.FirstOrDefaultAsync(m => m.Id == id, token)
            : await _context.Monedas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, token);
    public async Task<List<ComboDto>> ObtenerCombo(CancellationToken token) => await _context.Monedas
        .AsNoTracking()
        .Select(m => new ComboDto
        {
            Id = m.Id,
            Nombre = m.Nombre
        })
        .ToListAsync(token);
    public async Task<int> Crear(Moneda entity, CancellationToken token) { _context.Monedas.Add(entity); await _context.SaveChangesAsync(token); return entity.Id; }
    public async Task Actualizar(CancellationToken token) => await _context.SaveChangesAsync(token);
    public async Task Eliminar(Moneda entity, CancellationToken token) { _context.Monedas.Remove(entity); await _context.SaveChangesAsync(token); }

    public async Task<bool> TieneDependencias(Moneda moneda, CancellationToken token)
    {
        var existeEnPaises = await _context.Paises
            .AsNoTracking()
            .AnyAsync(p => p.CodigoMoneda == moneda.CodigoISO, token);

        if (existeEnPaises)
            return true;

        var existeEnEmpresas = await _context.Empresas
            .AsNoTracking()
            .AnyAsync(e => e.MonedaBaseId == moneda.Id, token);

        if (existeEnEmpresas)
            return true;

        var existeEnListaPrecio = await _context.ListasPrecios
            .AsNoTracking()
            .AnyAsync(e => e.MonedaId == moneda.Id, token);

        return existeEnListaPrecio;
    }
}
