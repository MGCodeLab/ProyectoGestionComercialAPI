using FluentValidation;

namespace Application.Features.Catalogo.UnidadMedida.ActualizarEstado;

public class ActualizarEstadoUnidadMedidaValidator : AbstractValidator<ActualizarEstadoUnidadMedidaCommand>
{
    public ActualizarEstadoUnidadMedidaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
