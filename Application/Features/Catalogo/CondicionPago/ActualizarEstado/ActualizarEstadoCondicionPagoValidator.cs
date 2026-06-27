using FluentValidation;

namespace Application.Features.Catalogo.CondicionPago.ActualizarEstado.CondicionPago;

public class ActualizarEstadoCondicionPagoValidator : AbstractValidator<ActualizarEstadoCondicionPagoCommand>
{
    public ActualizarEstadoCondicionPagoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
