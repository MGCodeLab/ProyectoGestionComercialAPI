using Domain.Catalogo;

namespace Application.Interfaces;

public interface ICondicionPagoService
{
    Task<List<CondicionPago>> ObtenerTodos(CancellationToken token);
    Task<CondicionPago?> ObtenerPorId(int id, CancellationToken token);
    Task<int> Crear(CondicionPago entity, CancellationToken token);
    Task<int> Actualizar(CondicionPago entity, CancellationToken token);
    Task<int> ActualizarEstado(int id, bool activo, CancellationToken token);
    Task<int> Eliminar(int id, CancellationToken token);
}
