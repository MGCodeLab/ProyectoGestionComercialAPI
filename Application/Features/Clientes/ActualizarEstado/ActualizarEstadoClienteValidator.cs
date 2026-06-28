using FluentValidation;

namespace Application.Features.Clientes.ActualizarEstado;

public class ActualizarEstadoClienteValidator : AbstractValidator<ActualizarEstadoClienteCommand>
{
    public ActualizarEstadoClienteValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
