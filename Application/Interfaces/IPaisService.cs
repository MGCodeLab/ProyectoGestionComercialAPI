using Application.Dtos;
using Domain.Catalogo;

namespace Application.Interfaces;

public interface IPaisService
{
    Task<List<Pais>> ObtenerTodos(CancellationToken token);
    Task<Pais?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
    Task<List<ComboDto>> ObtenerCombo(CancellationToken token);
    Task<int> Crear(Pais entity, CancellationToken token);
    Task Actualizar(CancellationToken token);
    Task Eliminar(Pais entity, CancellationToken token);
    Task<bool> TieneDependencias(Pais pais, CancellationToken token);
}
