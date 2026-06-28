using FluentValidation;

namespace Application.Features.Catalogo.ListaPrecio.ActualizarEstado;

public class ActualizarEstadoListaPrecioValidator : AbstractValidator<ActualizarEstadoListaPrecioCommand>
{
    public ActualizarEstadoListaPrecioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
