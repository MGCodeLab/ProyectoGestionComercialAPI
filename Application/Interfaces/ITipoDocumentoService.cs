using Domain.Catalogo;

namespace Application.Interfaces;

public interface ITipoDocumentoService
{
    Task<List<TipoDocumento>> ObtenerTodos(CancellationToken token);
    Task<TipoDocumento?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
    Task<int> Crear(TipoDocumento entity, CancellationToken token);
    Task Actualizar(CancellationToken token);
    Task Eliminar(TipoDocumento entity, CancellationToken token);
    Task<bool> TieneDependencias(TipoDocumento entity, CancellationToken token);
}
