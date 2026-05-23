using Application.Interfaces;
using Domain.Comercial;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ProveedorService : IProveedorService
{
    private readonly AppDbContext _context;

    public ProveedorService(AppDbContext context) => _context = context;

    public async Task<List<Proveedor>> ObtenerTodos(CancellationToken token)
        => await _context.Proveedores.Include(x => x.TipoDocumento).Include(x => x.Pais).ToListAsync(token);

    public async Task<Proveedor?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
       => (isAsTracking) ?
        await _context.Proveedores.FirstOrDefaultAsync(x => x.Id == id, token) :
        await _context.Proveedores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<Proveedor?> ObtenerPorIdConRelaciones(int id, CancellationToken token)
    {
        return await _context.Proveedores
            .AsNoTracking()
            .Include(x => x.TipoDocumento)
            .Include(x => x.Pais)
            .FirstOrDefaultAsync(x => x.Id == id, token);
    }


    public async Task<int> Crear(Proveedor entity, CancellationToken token)
    {
        _context.Proveedores.Add(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> Actualizar(Proveedor entity, CancellationToken token)
    {
        _context.Proveedores.Update(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> ActualizarEstado(int id, bool activo, CancellationToken token)
    {
        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(x => x.Id == id, token);
        if (proveedor == null)
            throw new InvalidOperationException($"Proveedor con ID {id} no encontrado");

        proveedor.Activo = activo;
        proveedor.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(token);
        return proveedor.Id;
    }

    public async Task<int> Eliminar(int id, CancellationToken token)
    {
        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(x => x.Id == id, token);
        if (proveedor == null)
            throw new InvalidOperationException($"Proveedor con ID {id} no encontrado");

        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync(token);
        return id;
    }
}
