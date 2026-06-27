using FluentValidation;

namespace Application.Features.Catalogo.CategoriaProducto.Eliminar.CategoriaProducto;

public class EliminarCategoriaProductoValidator : AbstractValidator<EliminarCategoriaProductoCommand>
{
    public EliminarCategoriaProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
