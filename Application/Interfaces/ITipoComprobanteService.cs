using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ITipoComprobanteService
    {
        Task<List<TipoComprobante>> ObtenerTodosAsync();
        Task<TipoComprobante> ObtenerPorIdAsync(int id);
        Task Crear(TipoComprobante tipoComprobante);
        Task Actualizar(TipoComprobante tipoComprobante);
        Task Eliminar(int id);
    }
}
