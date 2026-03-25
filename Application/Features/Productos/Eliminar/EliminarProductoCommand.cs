using MediatR;

namespace Application.Features.Productos.Eliminar.Commands
{
    public record EliminarProductoCommand(
        int Id
    ) : IRequest<Unit>;
}
