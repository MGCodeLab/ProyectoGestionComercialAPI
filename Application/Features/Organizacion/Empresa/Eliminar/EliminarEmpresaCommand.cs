using MediatR;

namespace Application.Features.Organizacion.Empresa.Eliminar
{
    public record EliminarEmpresaCommand(int Id) : IRequest<int>;
}
