using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface IAlmacenService
    {
        Task<Almacen?> ObtenerPorId(int id, bool tracking = false);
        Task<List<Almacen>> ObtenerTodos();
        Task<int> Crear(Almacen almacen);
        Task Actualizar(Almacen almacen);
        Task Eliminar(int id);
    }
}
