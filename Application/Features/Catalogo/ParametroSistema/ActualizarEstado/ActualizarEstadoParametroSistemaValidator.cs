using FluentValidation;

namespace Application.Features.Catalogo.ParametroSistema.ActualizarEstado.ParametroSistema;

public class ActualizarEstadoParametroSistemaValidator : AbstractValidator<ActualizarEstadoParametroSistemaCommand>
{
    public ActualizarEstadoParametroSistemaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
