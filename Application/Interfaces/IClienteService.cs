using Domain.Comercial;

namespace Application.Interfaces
{
    public interface IClienteService
    {
        Task<List<Cliente>> ObtenerTodos(CancellationToken token);
        Task<Cliente?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task<int> Crear(Cliente cliente, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar(Cliente cliente, CancellationToken token);
    }
}
