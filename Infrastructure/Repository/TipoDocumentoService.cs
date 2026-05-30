using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class TipoDocumentoService : ITipoDocumentoService
{
    private readonly AppDbContext _context;

    public TipoDocumentoService(AppDbContext context) => _context = context;

    public async Task<List<TipoDocumento>> ObtenerTodos(CancellationToken token)
        => await _context.TipoDocumentos.AsNoTracking().ToListAsync(token);

    public async Task<TipoDocumento?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
        => isAsTracking
            ? await _context.TipoDocumentos.FirstOrDefaultAsync(u => u.Id == id, token)
            : await _context.TipoDocumentos.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, token);

    public async Task<int> Crear(TipoDocumento entity, CancellationToken token)
    {
        _context.TipoDocumentos.Add(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task Actualizar(CancellationToken token)
        => await _context.SaveChangesAsync(token);

    public async Task Eliminar(TipoDocumento entity, CancellationToken token)
    {
        _context.TipoDocumentos.Remove(entity);
        await _context.SaveChangesAsync(token);
    }

    public async Task<bool> TieneDependencias(TipoDocumento entity, CancellationToken token)
    {
        var existeEnEmpresas = await _context.Empresas
            .AsNoTracking()
            .AnyAsync(e => e.TipoDocumentoId == entity.Id, token);
        if (existeEnEmpresas)
            return true;

        var existeEnProveedores = await _context.Proveedores
            .AsNoTracking()
            .AnyAsync(p => p.TipoDocumentoId == entity.Id, token);
        return existeEnProveedores;
    }
}
