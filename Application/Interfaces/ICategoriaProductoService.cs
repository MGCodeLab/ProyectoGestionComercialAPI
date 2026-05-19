using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ICategoriaProductoService
    {
        Task<List<CategoriaProducto>> ObtenerTodosAsync();
        Task<CategoriaProducto?> ObtenerPorIdAsync(int id, bool tracking = false);
        Task<List<CategoriaProducto>> ObtenerRaicesAsync();
        Task Crear(CategoriaProducto categoria);
        Task Actualizar(CategoriaProducto categoria);
        Task Eliminar(int id);
    }
}
