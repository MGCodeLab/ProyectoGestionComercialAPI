using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ISerieDocumentoService
    {
        Task<List<SerieDocumento>> ObtenerTodos(CancellationToken token);
        Task<SerieDocumento?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task<int> Crear(SerieDocumento entity, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar(SerieDocumento entity, CancellationToken token);
        Task<int> ObtenerProximoNumeroAsync(int serieDocumentoId, CancellationToken ct = default);
    }
}
