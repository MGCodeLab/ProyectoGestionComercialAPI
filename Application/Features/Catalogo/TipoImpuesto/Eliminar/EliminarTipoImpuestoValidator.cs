using FluentValidation;

namespace Application.Features.Catalogo.TipoImpuesto.Eliminar.TipoImpuesto;

public class EliminarTipoImpuestoValidator : AbstractValidator<EliminarTipoImpuestoCommand>
{
    public EliminarTipoImpuestoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
