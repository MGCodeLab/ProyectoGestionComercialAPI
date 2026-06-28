using FluentValidation;

namespace Application.Features.Catalogo.MarcaProducto.Eliminar;

public class EliminarMarcaProductoValidator : AbstractValidator<EliminarMarcaProductoCommand>
{
    public EliminarMarcaProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
