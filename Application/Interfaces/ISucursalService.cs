using Application.Dtos;
using Application.Dtos.Organizacion;
using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface ISucursalService
    {
        Task<Sucursal?> ObtenerPorId(int id, bool tracking = false);
        Task<List<Sucursal>> ObtenerTodos();
        Task<List<SucursalDto>> ObtenerTodosOptimizado();
        Task<SucursalDto?> ObtenerPorIdOptimizado(int id);
        Task<List<ComboDto>> ObtenerCombo();
        Task<int> Crear(Sucursal sucursal);
        Task Actualizar(Sucursal sucursal);
        Task Eliminar(int id);
        Task<List<ComboDto>> ObtenerComboByIdEmpresa(int IdEmpresa, CancellationToken cancellationToken);
        Task<bool> TieneDependencias(Sucursal sucursal, CancellationToken token);
    }
}
