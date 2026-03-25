using MediatR;

namespace Application.Features.Productos.Crear.Commands
{
    public record CrearProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio
    ) : IRequest<int>;
}
