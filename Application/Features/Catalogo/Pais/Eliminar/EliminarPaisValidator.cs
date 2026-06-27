using FluentValidation;

namespace Application.Features.Catalogo.Pais.Eliminar.Pais;

public class EliminarPaisValidator : AbstractValidator<EliminarPaisCommand>
{
    public EliminarPaisValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
