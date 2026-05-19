using Domain.Catalogo;

namespace Application.Interfaces
{
    public interface ITipoImpuestoService
    {
        Task<List<TipoImpuesto>> ObtenerTodosAsync();
        Task<TipoImpuesto> ObtenerPorIdAsync(int id);
        Task Crear(TipoImpuesto tipoImpuesto);
        Task Actualizar(TipoImpuesto tipoImpuesto);
        Task Eliminar(int id);
    }
}
