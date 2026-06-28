using Application.Dtos;
using Application.Dtos.Organizacion;
using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface IEmpresaService
    {
        Task<Empresa?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken);
        Task<Empresa?> ObtenerPrimera(CancellationToken cancellationToken);
        Task<List<Empresa>> ObtenerTodos(CancellationToken cancellationToken);
        Task<List<ComboDto>> ObtenerCombo(CancellationToken cancellationToken);
        Task<int> Crear(Empresa empresa, CancellationToken cancellationToken);
        Task Actualizar(Empresa empresa, CancellationToken cancellationToken);
        Task Eliminar(int id, CancellationToken cancellationToken);
        Task<bool> TieneDependencias(Empresa entity, CancellationToken token);
    }
}
