using Domain.Organizacion;

namespace Application.Interfaces
{
    public interface IEmpresaService
    {
        Task<Empresa?> ObtenerPorId(int id, bool tracking = false);
        Task<Empresa?> ObtenerPrimera();
        Task<List<Empresa>> ObtenerTodos();
        Task<int> Crear(Empresa empresa);
        Task Actualizar(Empresa empresa);
        Task Eliminar(int id);
    }
}
