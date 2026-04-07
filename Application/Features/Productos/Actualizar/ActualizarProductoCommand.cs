using MediatR;

namespace Application.Features.Productos.Actualizar
{
    public record ActualizarProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Id = 0
    ) : IRequest<Unit>;
}
