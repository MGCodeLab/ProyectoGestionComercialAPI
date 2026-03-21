using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodos(CancellationToken token);
        Task<Producto?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task Crear(Producto producto, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar(Producto producto, CancellationToken token);
    }
}
