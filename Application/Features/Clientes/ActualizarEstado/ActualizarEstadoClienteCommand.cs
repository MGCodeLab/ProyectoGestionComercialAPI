using MediatR;

namespace Application.Features.Clientes.ActualizarEstado
{
    public record ActualizarEstadoClienteCommand(int Id, bool Activo) : IRequest<Unit>;
}
