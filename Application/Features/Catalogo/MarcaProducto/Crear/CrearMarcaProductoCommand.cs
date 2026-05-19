using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Crear
{
    public record CrearMarcaProductoCommand(
        string Nombre,
        string? Descripcion,
        string? LogoUrl) : IRequest<int>;
}
