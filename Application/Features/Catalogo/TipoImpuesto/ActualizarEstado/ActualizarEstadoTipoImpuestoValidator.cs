using FluentValidation;

namespace Application.Features.Catalogo.TipoImpuesto.ActualizarEstado;

public class ActualizarEstadoTipoImpuestoValidator : AbstractValidator<ActualizarEstadoTipoImpuestoCommand>
{
    public ActualizarEstadoTipoImpuestoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
