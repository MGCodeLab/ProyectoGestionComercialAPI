using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ICategoriaProductoService
    {
        Task<List<CategoriaProducto>> ObtenerTodosAsync(CancellationToken cancellationToken);
        Task<CategoriaProducto?> ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken);
        Task<List<CategoriaProducto>> ObtenerRaicesAsync(CancellationToken cancellationToken);
        Task Crear(CategoriaProducto categoria, CancellationToken cancellationToken);
        Task Actualizar(CategoriaProducto categoria, CancellationToken cancellationToken);
        Task Eliminar(int id, CancellationToken cancellationToken);
    }
}
