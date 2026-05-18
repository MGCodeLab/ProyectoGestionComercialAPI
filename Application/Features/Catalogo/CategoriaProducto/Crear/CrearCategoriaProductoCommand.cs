using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Crear
{
    public record CrearCategoriaProductoCommand(
        string Nombre,
        string? Descripcion,
        int? CategoriaPadreId) : IRequest<int>;
}
