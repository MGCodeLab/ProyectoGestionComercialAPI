using FluentValidation;

namespace Application.Features.Organizacion.Sucursal.ActualizarEstado;

public class ActualizarEstadoSucursalValidator : AbstractValidator<ActualizarEstadoSucursalCommand>
{
    public ActualizarEstadoSucursalValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
