using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Eliminar
{
    public record EliminarCategoriaProductoCommand(
        int Id) : IRequest<int>;
}
