using FluentValidation;

namespace Application.Features.Comercial.Proveedor.Eliminar;

public class EliminarProveedorValidator : AbstractValidator<EliminarProveedorCommand>
{
    public EliminarProveedorValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
