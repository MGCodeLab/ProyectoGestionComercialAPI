using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Actualizar
{
    public record ActualizarMarcaProductoCommand(
        int Id,
        string Nombre,
        string? Descripcion,
        string? LogoUrl) : IRequest<int>;
}
