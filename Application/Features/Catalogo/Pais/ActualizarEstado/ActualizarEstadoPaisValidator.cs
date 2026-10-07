using FluentValidation;

namespace Application.Features.Catalogo.Pais.ActualizarEstado;

public class ActualizarEstadoPaisValidator : AbstractValidator<ActualizarEstadoPaisCommand>
{
    public ActualizarEstadoPaisValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
