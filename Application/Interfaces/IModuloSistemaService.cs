using Domain.Configuracion;
namespace Application.Interfaces;
public interface IModuloSistemaService { 
    Task<List<ModuloSistema>> ObtenerTodos(CancellationToken token); 
    Task<ModuloSistema?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token); 
    Task<int> Crear(ModuloSistema entity, CancellationToken token); 
    Task Actualizar(CancellationToken token);
    Task Eliminar(ModuloSistema entity, CancellationToken token); 
}


