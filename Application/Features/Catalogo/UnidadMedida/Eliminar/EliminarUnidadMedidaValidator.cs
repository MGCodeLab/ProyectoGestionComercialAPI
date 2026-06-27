using FluentValidation;

namespace Application.Features.Catalogo.UnidadMedida.Eliminar.UnidadMedida;

public class EliminarUnidadMedidaValidator : AbstractValidator<EliminarUnidadMedidaCommand>
{
    public EliminarUnidadMedidaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
