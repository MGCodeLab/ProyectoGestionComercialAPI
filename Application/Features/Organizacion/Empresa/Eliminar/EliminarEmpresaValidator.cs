using FluentValidation;

namespace Application.Features.Organizacion.Empresa.Eliminar;

public class EliminarEmpresaValidator : AbstractValidator<EliminarEmpresaCommand>
{
    public EliminarEmpresaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
