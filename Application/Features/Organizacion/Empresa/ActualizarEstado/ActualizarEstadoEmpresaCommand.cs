using MediatR;

namespace Application.Features.Organizacion.Empresa.ActualizarEstado
{
    public record ActualizarEstadoEmpresaCommand(
        int Id,
        bool Activo
    ) : IRequest<int>;
}
