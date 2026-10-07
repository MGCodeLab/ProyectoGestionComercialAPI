using FluentValidation;

namespace Application.Features.Productos.ActualizarEstado;

public class ActualizarEstadoProductoValidator : AbstractValidator<ActualizarEstadoProductoCommand>
{
    public ActualizarEstadoProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
