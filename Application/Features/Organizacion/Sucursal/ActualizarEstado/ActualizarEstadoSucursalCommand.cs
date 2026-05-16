using MediatR;

namespace Application.Features.Organizacion.Sucursal.ActualizarEstado
{
    public record ActualizarEstadoSucursalCommand(
        int Id,
        bool Activo
    ) : IRequest<int>;
}
