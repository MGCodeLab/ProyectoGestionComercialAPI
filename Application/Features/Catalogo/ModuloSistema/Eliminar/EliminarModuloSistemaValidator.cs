using FluentValidation;

namespace Application.Features.Catalogo.ModuloSistema.Eliminar;

public class EliminarModuloSistemaValidator : AbstractValidator<EliminarModuloSistemaCommand>
{
    public EliminarModuloSistemaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
