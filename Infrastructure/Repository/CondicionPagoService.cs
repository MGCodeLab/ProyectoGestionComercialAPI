using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class CondicionPagoService : ICondicionPagoService
{
    private readonly AppDbContext _context;

    public CondicionPagoService(AppDbContext context) => _context = context;

    public async Task<List<CondicionPago>> ObtenerTodos(CancellationToken token)
        => await _context.CondicionesPago.AsNoTracking().ToListAsync(token);

    public async Task<CondicionPago?> ObtenerPorId(int id, CancellationToken token)
        => await _context.CondicionesPago.FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<int> Crear(CondicionPago entity, CancellationToken token)
    {
        _context.CondicionesPago.Add(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> Actualizar(CondicionPago entity, CancellationToken token)
    {
        _context.CondicionesPago.Update(entity);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<int> ActualizarEstado(int id, bool activo, CancellationToken token)
    {
        var condicion = await _context.CondicionesPago.FirstOrDefaultAsync(x => x.Id == id, token);
        if (condicion == null)
            throw new InvalidOperationException($"Condición de pago con ID {id} no encontrada");

        condicion.Activo = activo;
        condicion.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync(token);
        return condicion.Id;
    }

    public async Task<int> Eliminar(int id, CancellationToken token)
    {
        var condicion = await _context.CondicionesPago.FirstOrDefaultAsync(x => x.Id == id, token);
        if (condicion == null)
            throw new InvalidOperationException($"Condición de pago con ID {id} no encontrada");

        _context.CondicionesPago.Remove(condicion);
        await _context.SaveChangesAsync(token);
        return id;
    }
}
