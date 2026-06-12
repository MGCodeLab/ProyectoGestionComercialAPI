using Application.Dtos;
using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ITipoComprobanteService
    {
        Task<List<TipoComprobante>> ObtenerTodos(CancellationToken token);
        Task<TipoComprobante?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task<int> Crear(TipoComprobante entity, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar(TipoComprobante entity, CancellationToken token);
        Task<List<ComboDto>> ObtenerCombo(CancellationToken token);
    }
}
