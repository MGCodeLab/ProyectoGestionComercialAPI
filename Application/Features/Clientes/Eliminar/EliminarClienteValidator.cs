using FluentValidation;

namespace Application.Features.Clientes.Eliminar;

public class EliminarClienteValidator : AbstractValidator<EliminarClienteCommand>
{
    public EliminarClienteValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
