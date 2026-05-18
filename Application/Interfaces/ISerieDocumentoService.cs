using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ISerieDocumentoService
    {
        Task<List<SerieDocumento>> ObtenerTodosAsync();
        Task<SerieDocumento> ObtenerPorIdAsync(int id);
        Task Crear(SerieDocumento serieDocumento);
        Task Actualizar(SerieDocumento serieDocumento);
        Task Eliminar(int id);
        Task<int> ObtenerProximoNumeroAsync(int serieDocumentoId, CancellationToken ct = default);
    }
}
