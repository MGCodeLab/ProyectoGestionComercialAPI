using FluentValidation;

namespace Application.Features.Catalogo.Moneda.Eliminar;

public class EliminarMonedaValidator : AbstractValidator<EliminarMonedaCommand>
{
    public EliminarMonedaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
