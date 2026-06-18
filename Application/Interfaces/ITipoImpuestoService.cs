using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ITipoImpuestoService
    {
        Task<List<TipoImpuesto>> ObtenerTodos(CancellationToken token);
        Task<TipoImpuesto?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task<int> Crear(TipoImpuesto entity, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar(TipoImpuesto entity, CancellationToken token);
    }
}
