using Domain.Catalogo;

namespace Application.Interfaces;

public interface IListaPrecioService
{
    Task<List<ListaPrecio>> ObtenerTodos(CancellationToken token);
    Task<ListaPrecio?> ObtenerPorId(int id, CancellationToken token);
    Task<ListaPrecio?> ObtenerDefaultAsync(CancellationToken token);
    Task<int> Crear(ListaPrecio entity, CancellationToken token);
    Task<int> Actualizar(ListaPrecio entity, CancellationToken token);
    Task<int> ActualizarEstado(int id, bool activo, CancellationToken token);
    Task<int> Eliminar(int id, CancellationToken token);
}
