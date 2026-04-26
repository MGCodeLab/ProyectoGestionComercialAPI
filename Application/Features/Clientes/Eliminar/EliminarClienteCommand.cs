using MediatR;

namespace Application.Features.Clientes.Eliminar
{
    public record EliminarClienteCommand(int Id) : IRequest<Unit>;
}
