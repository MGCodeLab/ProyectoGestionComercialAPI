using Domain.Configuracion;
namespace Application.Interfaces;
public interface IParametroSistemaService { 
    Task<List<ParametroSistema>> ObtenerTodos(CancellationToken token); 
    Task<ParametroSistema?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token); 
    Task<int> Crear(ParametroSistema entity, CancellationToken token); 
    Task Actualizar(CancellationToken token); 
    Task Eliminar(ParametroSistema entity, CancellationToken token); 
}
