using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.ActualizarEstado
{
    public record ActualizarEstadoMarcaProductoCommand(
        int Id,
        bool Activo) : IRequest<int>;
}
