using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface ISucursalService
    {
        Task<Sucursal?> ObtenerPorId(int id, bool tracking = false);
        Task<List<Sucursal>> ObtenerTodos();
        Task<int> Crear(Sucursal sucursal);
        Task Actualizar(Sucursal sucursal);
        Task Eliminar(int id);
    }
}
