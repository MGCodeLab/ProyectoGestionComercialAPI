using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface IMarcaProductoService
    {
        Task<List<MarcaProducto>> ObtenerTodosAsync(CancellationToken cancellationToken);
        Task<MarcaProducto?> ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken);
        Task Crear(MarcaProducto marca, CancellationToken cancellationToken);
        Task Actualizar(MarcaProducto marca, CancellationToken cancellationToken);
        Task Eliminar(int id, CancellationToken cancellationToken);
    }
}
