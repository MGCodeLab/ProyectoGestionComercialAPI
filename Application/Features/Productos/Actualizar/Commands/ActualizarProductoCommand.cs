using MediatR;

namespace Application.Features.Productos.Actualizar.Commands
{
    public record ActualizarProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    bool Activo,
    int Id = 0
    ) : IRequest<Unit>;
}
