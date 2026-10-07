using FluentValidation;

namespace Application.Features.Comercial.Proveedor.ActualizarEstado;

public class ActualizarEstadoProveedorValidator : AbstractValidator<ActualizarEstadoProveedorCommand>
{
    public ActualizarEstadoProveedorValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
