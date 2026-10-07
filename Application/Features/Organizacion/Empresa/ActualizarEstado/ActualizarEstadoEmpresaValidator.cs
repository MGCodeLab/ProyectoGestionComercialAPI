using FluentValidation;

namespace Application.Features.Organizacion.Empresa.ActualizarEstado;

public class ActualizarEstadoEmpresaValidator : AbstractValidator<ActualizarEstadoEmpresaCommand>
{
    public ActualizarEstadoEmpresaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
