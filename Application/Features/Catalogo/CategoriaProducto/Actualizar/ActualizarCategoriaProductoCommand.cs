using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Actualizar
{
    public record ActualizarCategoriaProductoCommand(
        int Id,
        string Nombre,
        string? Descripcion,
        int? CategoriaPadreId) : IRequest<int>;
}
