using Domain.Catalogo;

namespace Application.Interfaces;

public interface IMonedaService
{
    Task<List<Moneda>> ObtenerTodos(CancellationToken token);
    Task<Moneda?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
    Task<int> Crear(Moneda entity, CancellationToken token);
    Task Actualizar(CancellationToken token);
    Task Eliminar(Moneda entity, CancellationToken token);
}
