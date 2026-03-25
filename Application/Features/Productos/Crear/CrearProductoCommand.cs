using MediatR;

namespace Application.Features.Productos.Crear
{
    public record CrearProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio
    ) : IRequest<int>;
}
