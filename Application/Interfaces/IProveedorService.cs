using Domain.Comercial;

namespace Application.Interfaces;

public interface IProveedorService
{
    Task<List<Proveedor>> ObtenerTodos(CancellationToken token);
    Task<Proveedor?> ObtenerPorId(int id, CancellationToken token);
    Task<int> Crear(Proveedor entity, CancellationToken token);
    Task<int> Actualizar(Proveedor entity, CancellationToken token);
    Task<int> ActualizarEstado(int id, bool activo, CancellationToken token);
    Task<int> Eliminar(int id, CancellationToken token);
}
