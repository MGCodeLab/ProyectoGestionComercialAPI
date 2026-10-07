using FluentValidation;

namespace Application.Features.Catalogo.Moneda.ActualizarEstado;

public class ActualizarEstadoMonedaValidator : AbstractValidator<ActualizarEstadoMonedaCommand>
{
    public ActualizarEstadoMonedaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
