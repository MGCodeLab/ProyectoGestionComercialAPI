using Application.Dtos;
using Application.Dtos.Organizacion;
using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface ISucursalService
    {
        Task<Sucursal?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken);
        Task<List<Sucursal>> ObtenerTodos(CancellationToken cancellationToken);
        Task<List<SucursalDto>> ObtenerTodosOptimizado(CancellationToken cancellationToken);
        Task<SucursalDto?> ObtenerPorIdOptimizado(int id, CancellationToken cancellationToken);
        Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken);
        Task<int> Crear(Sucursal sucursal, CancellationToken cancellationToken);
        Task Actualizar(Sucursal sucursal, CancellationToken cancellationToken);
        Task Eliminar(int id, CancellationToken cancellationToken);
        Task<List<ComboDto>> ObtenerComboByIdEmpresa(int IdEmpresa, CancellationToken cancellationToken);
        Task<bool> TieneDependencias(Sucursal sucursal, CancellationToken token);
    }
}
