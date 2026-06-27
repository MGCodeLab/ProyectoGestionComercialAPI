using FluentValidation;

namespace Application.Features.Organizacion.Almacen.ActualizarEstado;

public class ActualizarEstadoAlmacenValidator : AbstractValidator<ActualizarEstadoAlmacenCommand>
{
    public ActualizarEstadoAlmacenValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
