using FluentValidation;

namespace Application.Features.Organizacion.Sucursal.Eliminar;

public class EliminarSucursalValidator : AbstractValidator<EliminarSucursalCommand>
{
    public EliminarSucursalValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
