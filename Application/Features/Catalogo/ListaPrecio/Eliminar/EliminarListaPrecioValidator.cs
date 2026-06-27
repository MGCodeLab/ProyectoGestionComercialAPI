using FluentValidation;

namespace Application.Features.Catalogo.ListaPrecio.Eliminar;

public class EliminarListaPrecioValidator : AbstractValidator<EliminarListaPrecioCommand>
{
    public EliminarListaPrecioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
