using MediatR;

namespace Application.Features.Organizacion.Almacen.ActualizarEstado
{
    public record ActualizarEstadoAlmacenCommand(
        int Id,
        bool Activo
    ) : IRequest<int>;
}
