using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ListaPrecioService : IListaPrecioService
{
    private readonly AppDbContext _context;

    public ListaPrecioService(AppDbContext context) => _context = context;

    public async Task<List<ListaPrecio>> ObtenerTodos(CancellationToken token)
        => await _context.ListasPrecios.Include(x => x.Moneda).ToListAsync(token);

    public async Task<ListaPrecio?> ObtenerPorId(int id, CancellationToken token)
        => await _context.ListasPrecios.Include(x => x.Moneda).FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<ListaPrecio?> ObtenerDefaultAsync(CancellationToken token)
        => await _context.ListasPrecios.FirstOrDefaultAsync(x => x.EsDefault && x.Activo, token);

    public async Task<int> Crear(ListaPrecio entity, CancellationToken token)
    {
        _context.ListasPrecios.Add(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> Actualizar(ListaPrecio entity, CancellationToken token)
    {
        _context.ListasPrecios.Update(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> ActualizarEstado(int id, bool activo, CancellationToken token)
    {
        var lista = await _context.ListasPrecios.FirstOrDefaultAsync(x => x.Id == id, token);
        if (lista == null)
            throw new InvalidOperationException($"Lista de precios con ID {id} no encontrada");

        lista.Activo = activo;
        lista.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(token);
        return lista.Id;
    }

    public async Task<int> Eliminar(int id, CancellationToken token)
    {
        var lista = await _context.ListasPrecios.FirstOrDefaultAsync(x => x.Id == id, token);
        if (lista == null)
            throw new InvalidOperationException($"Lista de precios con ID {id} no encontrada");

        _context.ListasPrecios.Remove(lista);
        await _context.SaveChangesAsync(token);
        return id;
    }
}
