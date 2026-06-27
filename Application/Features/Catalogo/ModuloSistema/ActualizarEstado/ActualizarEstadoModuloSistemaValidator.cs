using FluentValidation;

namespace Application.Features.Catalogo.ModuloSistema.ActualizarEstado.ModuloSistema;

public class ActualizarEstadoModuloSistemaValidator : AbstractValidator<ActualizarEstadoModuloSistemaCommand>
{
    public ActualizarEstadoModuloSistemaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
