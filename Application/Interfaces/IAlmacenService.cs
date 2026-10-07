using Application.Dtos;
using Application.Dtos.Organizacion;
using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface IAlmacenService
    {
        Task<Almacen?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken);
        Task<List<Almacen>> ObtenerTodos(CancellationToken cancellationToken);
        Task<List<AlmacenDto>> ObtenerTodosOptimizado(CancellationToken cancellationToken);
        Task<AlmacenDto?> ObtenerPorIdOptimizado(int id, CancellationToken cancellationToken);
        Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken);
        Task<int> Crear(Almacen almacen, CancellationToken cancellationToken);
        Task Actualizar(Almacen almacen, CancellationToken cancellationToken);
        Task Eliminar(int id, CancellationToken cancellationToken);
    }
}
