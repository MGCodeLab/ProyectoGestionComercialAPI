using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface IMarcaProductoService
    {
        Task<List<MarcaProducto>> ObtenerTodosAsync();
        Task<MarcaProducto?> ObtenerPorIdAsync(int id, bool tracking = false);
        Task Crear(MarcaProducto marca);
        Task Actualizar(MarcaProducto marca);
        Task Eliminar(int id);
    }
}
