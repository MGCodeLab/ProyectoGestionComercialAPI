using Microsoft.EntityFrameworkCore;
using Application.Dtos;
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

    public async Task<List<ComboDto>> ObtenerCombo(CancellationToken token)
        => await _context.Paises
            .AsNoTracking()
            .Select(p => new ComboDto
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .ToListAsync(token);

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

    public async Task<bool> TieneDependencias(Pais pais, CancellationToken token)
    {
        // Empresa.PaisId
        var existeEnEmpresas = await _context.Empresas
            .AsNoTracking()
            .AnyAsync(e => e.PaisId == pais.Id, token);

        if (existeEnEmpresas)
            return true;

        // Sucursal.PaisId
        var existeEnSucursales = await _context.Sucursales
            .AsNoTracking()
            .AnyAsync(s => s.PaisId == pais.Id, token);

        if (existeEnSucursales)
            return true;

        // Proveedor.PaisId
        var existeEnProveedores = await _context.Proveedores
            .AsNoTracking()
            .AnyAsync(p => p.PaisId == pais.Id, token);

        return existeEnProveedores;
    }
}
