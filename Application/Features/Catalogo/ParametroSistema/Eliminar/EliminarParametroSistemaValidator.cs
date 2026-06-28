using FluentValidation;

namespace Application.Features.Catalogo.ParametroSistema.Eliminar;

public class EliminarParametroSistemaValidator : AbstractValidator<EliminarParametroSistemaCommand>
{
    public EliminarParametroSistemaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
