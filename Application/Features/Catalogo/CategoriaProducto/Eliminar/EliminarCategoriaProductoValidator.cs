using FluentValidation;

namespace Application.Features.Catalogo.CategoriaProducto.Eliminar;

public class EliminarCategoriaProductoValidator : AbstractValidator<EliminarCategoriaProductoCommand>
{
    public EliminarCategoriaProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
