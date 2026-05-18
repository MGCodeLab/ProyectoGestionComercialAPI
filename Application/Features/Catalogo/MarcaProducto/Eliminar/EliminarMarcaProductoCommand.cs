using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Eliminar
{
    public record EliminarMarcaProductoCommand(
        int Id) : IRequest<int>;
}
