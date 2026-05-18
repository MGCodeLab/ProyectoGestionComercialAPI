using MediatR;

namespace Application.Features.Productos.Actualizar
{
    public record ActualizarProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int? UnidadMedidaId = null,
    int? CategoriaProductoId = null,
    int? MarcaProductoId = null,
    int Id = 0
    ) : IRequest<Unit>;
}
