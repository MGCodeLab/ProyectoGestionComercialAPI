using Application.Dtos;
using Application.Dtos.Organizacion;
using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface IEmpresaService
    {
        Task<Empresa?> ObtenerPorId(int id, bool tracking = false);
        Task<Empresa?> ObtenerPrimera();
        Task<List<Empresa>> ObtenerTodos();
        Task<List<ComboDto>> ObtenerCombo();
        Task<int> Crear(Empresa empresa);
        Task Actualizar(Empresa empresa);
        Task Eliminar(int id);
        Task<bool> TieneDependencias(Empresa entity, CancellationToken token);
    }
}
