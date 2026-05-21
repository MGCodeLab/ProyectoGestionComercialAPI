using Domain.Catalogo;
namespace Application.Interfaces;
public interface IUnidadMedidaService { 
    Task<List<UnidadMedida>> ObtenerTodos(CancellationToken token); 
    Task<UnidadMedida?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token); 
    Task<int> Crear(UnidadMedida entity, CancellationToken token); 
    Task Actualizar(CancellationToken token);
    Task Eliminar(UnidadMedida entity, CancellationToken token);
    Task<bool> TieneDependencias(UnidadMedida entity, CancellationToken token);
}

