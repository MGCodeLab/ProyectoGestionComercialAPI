using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class PaisService : IPaisService
{
    private readonly AppDbContext _context;

    public PaisService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pais>> ObtenerTodos(CancellationToken token)
    {
        return await _context.Paises
            .AsNoTracking()
            .ToListAsync(token);
    }

    public async Task<Pais?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
    {
        var query = _context.Paises.AsQueryable();
        if (!isAsTracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(p => p.Id == id, token);
    }

    public async Task<int> Crear(Pais entity, CancellationToken token)
    {
        _context.Paises.Add(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task Actualizar(CancellationToken token)
    {
        await _context.SaveChangesAsync(token);
    }

    public async Task Eliminar(Pais entity, CancellationToken token)
    {
        _context.Paises.Remove(entity);
        await _context.SaveChangesAsync(token);
    }
}
