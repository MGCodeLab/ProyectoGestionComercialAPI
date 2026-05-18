using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.ActualizarEstado
{
    public record ActualizarEstadoCategoriaProductoCommand(
        int Id,
        bool Activo) : IRequest<int>;
}
