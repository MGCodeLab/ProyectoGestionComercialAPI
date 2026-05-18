using MediatR;

namespace Application.Features.Productos.Crear
{
    public record CrearProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int? UnidadMedidaId = null,
    int? CategoriaProductoId = null,
    int? MarcaProductoId = null
    ) : IRequest<int>;
}
