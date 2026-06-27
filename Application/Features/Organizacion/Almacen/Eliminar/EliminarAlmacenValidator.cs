using FluentValidation;

namespace Application.Features.Organizacion.Almacen.Eliminar.Almacen;

public class EliminarAlmacenValidator : AbstractValidator<EliminarAlmacenCommand>
{
    public EliminarAlmacenValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
